using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Containers
{
    public class DvGridLayoutPanel : DvContainer
    {
        #region Properteis
        public List<DvGridRow> Rows { get; private set; } = new List<DvGridRow>();
        public override DvGridLayoutControlCollection Controls => _lsControl;
        #endregion

        #region Member Variable
        private DvGridLayoutControlCollection _lsControl;
        private Dictionary<DvControl, DvGridIndex> dicLayout = new Dictionary<DvControl, DvGridIndex>();
        #endregion

        #region Constructor
        public DvGridLayoutPanel()
        {
            _lsControl = new DvGridLayoutControlCollection(this);
            _lsControl.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            OnLayout();

            base.OnDraw(Canvas);
        }
        #endregion
        #region OnLayout
        protected override void OnLayout()
        {
            var rt = new SKRect(0, 0, Width, Height);
            var vhs = GetRowsHeights(rt);

            foreach (var v in Controls.Values)
            {
                if (dicLayout.ContainsKey(v))
                {
                    var idx = dicLayout[v];

                    if (idx.Row >= 0 && idx.Row < Rows.Count)
                    {
                        var row = Rows[idx.Row];
                        var vws = GetColumnsWidth(rt, row);

                        var x = Convert.ToInt32(vws.pos[idx.Column]);
                        var y = Convert.ToInt32(vhs.pos[idx.Row]);
                        var w = Convert.ToInt32(vws.wh[idx.Column]);
                        var h = Convert.ToInt32(vhs.wh[idx.Row]);

                        v.X = Convert.ToInt32(x + v.Margin.Left);
                        v.Y = Convert.ToInt32(y + v.Margin.Top);
                        v.Width = Convert.ToInt32(w - (v.Margin.Left + v.Margin.Right));
                        v.Height = Convert.ToInt32(h - (v.Margin.Top + v.Margin.Bottom));
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Method
        #region SetCellPosition
        public void SetCellPosition(DvControl control, int column, int row)
        {
            if (!dicLayout.ContainsKey(control)) dicLayout.Add(control, new DvGridIndex() { Column = column, Row = row });
            else
            {
                dicLayout[control].Row = row;
                dicLayout[control].Column = column;
            }
        }
        #endregion
        #region GetCellPosition
        public DvGridIndex GetCellPosition(DvControl control)
        {
            if (dicLayout.ContainsKey(control)) return dicLayout[control];
            else return null;
        }
        #endregion

        #region GetColumnsWidths / GetRowHeights
        whinf GetColumnsWidth(SKRect rtContent, DvGridRow row)
        {
            var ret = new whinf();
            var tw = rtContent.Width;
            var cw = tw - row.Columns.Where(x => x.Mode == SizeMode.Pixel).Sum(x => x.Size);

            float x = rtContent.Left;
            ret.pos.Add(x);
            foreach (var v in row.Columns)
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


    #region class : DvGridIndex
    public class DvGridIndex
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }
    #endregion
    #region class : DvGridRow
    public class DvGridRow
    {
        public SizeMode Mode { get; set; } = SizeMode.Percent;
        public float Size { get; set; } = 50;

        public List<DvGridColumn> Columns { get; } = new List<DvGridColumn>();
    }
    #endregion
    #region class : DvGridColumn
    public class DvGridColumn
    {
        public SizeMode Mode { get; set; } = SizeMode.Percent;
        public float Size { get; set; } = 50;
    }
    #endregion
    
}
