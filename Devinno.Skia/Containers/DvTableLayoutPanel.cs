using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using Newtonsoft.Json;
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
        public List<SizeInfo> Columns { get; private set; } = new List<SizeInfo>();
        public List<SizeInfo> Rows { get; private set; } = new List<SizeInfo>();
        public override DvTableLayoutControlCollection Controls => _lsControl;

        [JsonIgnore]
        public Dictionary<DvControl, DvTableIndex> Indexes => dicLayout; 
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
        #region Draw
        protected override void OnDraw(SKCanvas Canvas)
        {
            OnLayout();

            base.OnDraw(Canvas);
        }
        #endregion

        #region Layout
        protected override void OnLayout()
        {
            var rt = new SKRect(0, 0, Width, Height);

            var rtv = Util.DevideSizeV(rt, Rows).ToList();
            var rth = Util.DevideSizeH(rt, Columns).ToList();

            foreach (var v in Controls.Values)
            {
                if (dicLayout.ContainsKey(v))
                {
                    var idx = dicLayout[v];

                    var x = rth[idx.Column].Left;
                    var y = rtv[idx.Row].Top;
                    var w = rth.GetRange(idx.Column, idx.ColSpan).Sum(x => x.Width);
                    var h = rtv.GetRange(idx.Row, idx.RowSpan).Sum(x => x.Height);

                    v.Bounds = Util.FromRect(Util.FromRect(x, y, w, h), v.Margin);
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
        #region Areas
        public void Areas(Action<int, int, SKRect> act)
        {
            var rt = new SKRect(0, 0, Width, Height);
            var rtvs = Util.DevideSizeVH(rt, Rows, Columns);

            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    var rtv = rtvs[i, j];
                    act(i, j, rtv);
                }
            }
        }
        #endregion
        #endregion
    }

    #region class : DvTableIndex
    public class DvTableIndex
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; } = 1;
        public int ColSpan { get; set; } = 1;
    }
    #endregion

}
