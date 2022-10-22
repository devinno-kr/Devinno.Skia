using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Containers
{
    public class DvTableLayoutPanel : DvContainer
    {
        #region Properteis
        public List<DvTableColumn> Columns { get; private set; } = new List<DvTableColumn>();
        public List<DvTableRow> Rows { get; private set; } = new List<DvTableRow>();
        public override DvTableLayoutControlCollection Controls => _lsControl;
        #endregion

        #region Member Variable
        private DvTableLayoutControlCollection _lsControl;
        private Dictionary<DvControl, DvTableIndex> dicLayout = new Dictionary<DvControl, DvTableIndex>();
        #endregion

        #region Constructor
        public DvTableLayoutPanel()
        {
            _lsControl = new DvTableLayoutControlCollection(this);
            _lsControl.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnLayout
        protected override void OnLayout()
        {
            var rt = new SKRect(0, 0, Width, Height);
            var vws = GetColumnsWidth(rt);
            var vhs = GetRowsHeights(rt);

            foreach (var v in Controls.Values)
            {
                if (dicLayout.ContainsKey(v))
                {
                    var idx = dicLayout[v];

                    var x = Convert.ToInt32(vws.pos[idx.Column]);
                    var y = Convert.ToInt32(vhs.pos[idx.Row]);
                    var w = Convert.ToInt32(vws.wh.GetRange(idx.Column, idx.ColSpan).Sum());
                    var h = Convert.ToInt32(vhs.wh.GetRange(idx.Row, idx.RowSpan).Sum());

                    v.X = Convert.ToInt32(x + v.Margin.Left);
                    v.Y = Convert.ToInt32(y + v.Margin.Top);
                    v.Width = Convert.ToInt32(w - (v.Margin.Left + v.Margin.Right));
                    v.Height = Convert.ToInt32(h - (v.Margin.Top + v.Margin.Bottom));
                }
            }
        }
        #endregion
        #endregion

        #region Method
        #region SetCellPosition
        public void SetCellPosition(DvControl control, int column, int row, int colspan = 1, int rowspan = 1)
        {
            if (!dicLayout.ContainsKey(control))
                dicLayout.Add(control, new DvTableIndex() { Column = column, Row = row, ColSpan = colspan, RowSpan = rowspan });
            else
            {
                dicLayout[control].Row = row;
                dicLayout[control].Column = column;
                dicLayout[control].ColSpan = colspan;
                dicLayout[control].RowSpan = rowspan;
            }
        }
        #endregion
        #region GetCellPosition
        public DvTableIndex GetCellPosition(DvControl control)
        {
            if (dicLayout.ContainsKey(control)) return dicLayout[control];
            else return null;
        }
        #endregion

        #region GetColumnsWidths / GetRowHeights
        whinf GetColumnsWidth(SKRect rtContent)
        {
            var ret = new whinf();
            var tw = rtContent.Width;
            var cw = tw - Columns.Where(x => x.Mode == SizeMode.Pixel).Sum(x => x.Size);
            
            float x = rtContent.Left;
            ret.pos.Add(x);
            foreach (var v in Columns)
            {
                var w = v.Mode == SizeMode.Pixel ? (v.Size) : (cw * (v.Size / 100F));
                x += w;

                ret.wh.Add(w);
                ret.pos.Add(x);
            }
            return ret;
        }
        whinf GetRowsHeights(SKRect rtContent)
        {
            var ret = new whinf();
            var th = rtContent.Height;
            var ch = th - Rows.Where(x => x.Mode == SizeMode.Pixel).Sum(x => x.Size);

            float y = rtContent.Top;
            ret.pos.Add(y);
            foreach (var v in Rows)
            {
                var h = v.Mode == SizeMode.Pixel ? (v.Size) : (ch * (v.Size / 100F));
                y += h;

                ret.wh.Add(h);
                ret.pos.Add(y);
            }
            return ret;
        }
        #endregion
        #endregion
    }

    #region enum : SizeMode
    public enum SizeMode { Pixel, Percent}
    #endregion
    #region class : DvTableIndex
    public class DvTableIndex
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; } = 1;
        public int ColSpan { get; set; } = 1;
    }
    #endregion
    #region class : DvTableRow
    public class DvTableRow
    {
        public SizeMode Mode { get; set; } = SizeMode.Percent;
        public float Size { get; set; } = 50;
    }
    #endregion
    #region class : DvTableColumn
    public class DvTableColumn
    {
        public SizeMode Mode { get; set; } = SizeMode.Percent;
        public float Size { get; set; } = 50;
    }
    #endregion
    #region class : whinf
    class whinf
    {
        public List<float> pos = new List<float>();
        public List<float> wh = new List<float>();
    }
    #endregion
}
