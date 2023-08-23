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
    public class DvGridLayoutPanel : DvContainer
    {
        #region Properties
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
            var rtv = Util.DevideSizeV(rt, Rows.Select(x => x as SizeInfo).ToList());

            foreach (var v in Controls.Values)
            {
                if (dicLayout.ContainsKey(v))
                {
                    var idx = dicLayout[v];

                    if (idx.Row >= 0 && idx.Row < Rows.Count)
                    {
                        var row = Rows[idx.Row];
                        if (row.Columns.Count > 0)
                        {
                            var rth = Util.DevideSizeH(rt, row.Columns);

                            var x = rth[idx.Column].Left;
                            var y = rtv[idx.Row].Top;
                            var w = rth[idx.Column].Width;
                            var h = rtv[idx.Row].Height;

                            v.Bounds = Util.FromRect(Util.FromRect(x, y, w, h), v.Margin);
                        }
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
    public class DvGridRow : SizeInfo
    {
        public DvGridRow(DvSizeMode mode, float size) : base(mode, size)
        {
        }

        public List<SizeInfo> Columns { get; } = new List<SizeInfo>();
    }
    #endregion
}
