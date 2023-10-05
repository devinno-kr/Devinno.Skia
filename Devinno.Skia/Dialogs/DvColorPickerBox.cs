using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvColorPickerBox : DvWindow
    {
        #region Properties
        public int ButtonTextSize { get; set; } = 12;
        public int ButtonIconSize { get; set; } = 16;

        public SKColor SelectedColor => SKColor.FromHsv(nH, nS, nV);
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvTableLayoutPanel tpnl;
        DvControl color, hue;
        DvLabel lbl;
        DvValueInputNumber<int> inR, inG, inB;
        DvButton btnOK;
        DvButton btnCancel;

        bool bOK = false;
        byte[] ba = new byte[256 * 256 * 4];

        float nH, nS, nV;
        bool bDownHue, bDownColor;
        bool ignore;
        #endregion

        #region Constructor
        public DvColorPickerBox()
        {
            IconString = "fa-palette";
            Width = 300;
            Height = 400;

            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = new Padding(10, TitleHeight + 10, 10, 10), Fill = true };

            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Pixel, 256 + 6));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Pixel, 10));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Pixel, 20));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Pixel, 10));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Pixel, 100));

            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 100));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));

            Controls.Add(tpnl);
            #endregion
            #region New
            color = new DvControl { Name = nameof(color) };
            hue = new DvControl { Name = nameof(hue) };
            lbl = new DvLabel { Name = nameof(lbl), Text = "" };
            inR = new DvValueInputNumber<int> { Name = nameof(inR), Title = "R", Minimum = 0, Maximum = 255, TitleAreaSize = 30 };
            inG = new DvValueInputNumber<int> { Name = nameof(inG), Title = "G", Minimum = 0, Maximum = 255, TitleAreaSize = 30 };
            inB = new DvValueInputNumber<int> { Name = nameof(inB), Title = "B", Minimum = 0, Maximum = 255, TitleAreaSize = 30 };
            btnOK = new DvButton { Name = nameof(btnOK), Text = "확인", Gradient = true };
            btnCancel = new DvButton { Name = nameof(btnCancel), Text = "취소", Gradient = true };
            #endregion
            #region Controls.Add
            tpnl.Controls.Add(color, 0, 0, 1, 7);
            tpnl.Controls.Add(hue, 2, 0, 1, 7);
            tpnl.Controls.Add(lbl, 4, 0);
            tpnl.Controls.Add(inR, 4, 1);
            tpnl.Controls.Add(inG, 4, 2);
            tpnl.Controls.Add(inB, 4, 3);
            tpnl.Controls.Add(btnOK, 4, 5);
            tpnl.Controls.Add(btnCancel, 4, 6);
            #endregion
            #region Event
            #region in[R/G/B].ValueChanged
            inR.ValueChanged += (o, s) =>
            {
                var c = SelectedColor.WithRed(Convert.ToByte(inR.Value));
                if (!ignore) { c.ToHsv(out nH, out nS, out nV); HueSet(); }
            };
            inG.ValueChanged += (o, s) =>
            {
                var c = SelectedColor.WithGreen(Convert.ToByte(inG.Value));
                if (!ignore) { c.ToHsv(out nH, out nS, out nV); HueSet(); }
            };
            inB.ValueChanged += (o, s) =>
            {
                var c = SelectedColor.WithBlue(Convert.ToByte(inB.Value));
                if (!ignore) c.ToHsv(out nH, out nS, out nV); HueSet();
            };
            #endregion
            #region btn[OK/Cancel].MouseClick
            btnCancel.MouseClick += (o, s) => { bOK = false; Hide(); };
            btnOK.MouseClick += (o, s) => { bOK = true; Hide(); };
            #endregion

            #region hue.Drawn
            hue.Drawn += (o, cv) =>
            {
                cv.DrawBitmap(ResourceTool.saturation, 0, 0);
                var hy = Convert.ToInt32(MathTool.Map(nH, 360, 0, 0, 255));
                using (var p = new SKPaint())
                {
                    p.IsStroke = true;
                    p.Color = SKColors.White; cv.DrawLine(0, hy, 20, hy, p);
                    p.Color = SKColors.Black; cv.DrawLine(0, hy - 1, 20, hy - 1, p); cv.DrawLine(0, hy + 1, 20, hy + 1, p);
                }
            };
            #endregion
            #region hue.MouseDown
            hue.MouseDown += (o, s) =>
            {
                bDownHue = true;

                nH = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.Y, 0, 255), 0, 255, 360, 0));
                HueSet();
            };
            #endregion
            #region hue.MouseMove
            hue.MouseMove += (o, s) =>
            {
                if (bDownHue)
                {
                    nH = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.Y, 0, 255), 0, 255, 360, 0));
                    HueSet();
                }
            };
            #endregion
            #region hue.MouseUp
            hue.MouseUp += (o, s) =>
            {
                if (bDownHue)
                {
                    nH = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.Y, 0, 255), 0, 255, 360, 0));
                    HueSet();

                    bDownHue = false;
                }
            };
            #endregion

            #region color.Drawn
            color.Drawn += (o, cv) =>
            {
                using (var bm = new SKBitmap(256, 256, SKColorType.Rgba8888, SKAlphaType.Premul))
                {
                    var handle = GCHandle.Alloc(ba, GCHandleType.Pinned);
                    var info = new SKImageInfo(256, 256, SKColorType.Rgba8888, SKAlphaType.Premul);
                    var map = new SKPixmap(info, handle.AddrOfPinnedObject());
                    bm.InstallPixels(map);
                    handle.Free();

                    cv.DrawBitmap(bm, 0, 0);
                }

                using (var p = new SKPaint() { IsAntialias = true })
                {
                    var x = Convert.ToInt32(MathTool.Map(nS, 0, 100, 0, 255)) + 0.5F;
                    var y = Convert.ToInt32(MathTool.Map(nV, 0, 100, 255, 0)) + 0.5F;

                    p.IsStroke = true;
                    p.StrokeWidth = 1;

                    p.Color = SKColors.Black;
                    cv.DrawLine(x - 4 + 1, y + 1, x + 4 + 1, y + 1, p);
                    cv.DrawLine(x + 1, y - 4 + 1, x + 1, y + 4 + 1, p);

                    p.Color = SKColors.White;
                    cv.DrawLine(x - 4, y, x + 4, y, p);
                    cv.DrawLine(x, y - 4, x, y + 4, p);

                }
            };
            #endregion
            #region color.MouseDown 
            color.MouseDown += (o, s) =>
            {
                bDownColor = true;

                nS = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.X, 0, 255), 0, 255, 0, 100));
                nV = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.Y, 0, 255), 0, 255, 100, 0));

                Set(true);
            };
            #endregion
            #region color.MouseUp
            color.MouseUp += (o, s) =>
            {
                if (bDownColor)
                {
                    nS = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.X, 0, 255), 0, 255, 0, 100));
                    nV = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.Y, 0, 255), 0, 255, 100, 0));

                    Set(true);

                    bDownColor = false;
                }
            };
            #endregion
            #region color.MouseMove
            color.MouseMove += (o, s) =>
            {
                if (bDownColor)
                {
                    nS = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.X, 0, 255), 0, 255, 0, 100));
                    nV = Convert.ToSingle(MathTool.Map(MathTool.Constrain(s.Y, 0, 255), 0, 255, 100, 0));

                    Set(true);
                }
            };
            #endregion
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
        #region Set
        void Set(bool set = false)
        {
            lbl.LabelColor = SelectedColor;
            if (set) ignore = true;
            inR.Value = SelectedColor.Red;
            inG.Value = SelectedColor.Green;
            inB.Value = SelectedColor.Blue;
            ignore = false;
        }
        #endregion
        #region HueSet
        void HueSet()
        {
            var H = 256;
            var W = 256;
            var r = Parallel.For(0, H * W, (iv) =>
            {
                int y = iv / W;
                int x = iv - (y * W);
                int numBytes = (y * (W * 4)) + (x * 4);

                var s = Convert.ToSingle(MathTool.Map(x, 0.0, 255, 0.0, 100.0));
                var v = Convert.ToSingle(MathTool.Map(y, 0.0, 255, 100.0, 0.0));
                var c = SKColor.FromHsv(nH, s, v);

                ba[numBytes] = c.Red;
                ba[numBytes + 1] = c.Green;
                ba[numBytes + 2] = c.Blue;
                ba[numBytes + 3] = c.Alpha;
            });

            Set(true);
        }
        #endregion

        #region show
        void show(string Title, Action act1, Action act2)
        {
            this.Width = 10 + (256 + 6) + 10 + 20 + 10 + 100 + 10;
            this.Height = TitleHeight + 10 + (256 + 6) + 10;

            this.Title = Title;

            act1();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) => { Thread.Sleep(DvDesign.HIDE_TIME); act2(); });
            });
        }
        #endregion

        #region ShowColorPicker
        public void ShowColorPicker(string Title, SKColor? value, Action<SKColor?> result)
        {
            var vc = value ?? SKColors.White;
            vc.ToHsv(out nH, out nS, out nV);

            show(Title, () =>
            {
                HueSet();

            }, () =>
            {
                if (bOK) result(SelectedColor);
                else result(null);
            });
        }
        #endregion
        #endregion
    }
}
