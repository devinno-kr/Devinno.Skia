using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;

namespace SampleRPi.Pages
{
    public partial class PageTable : DvPage
    {
        #region Member Variable
        List<GridItem> ls = new List<GridItem>();
        Random rnd = new Random();
        #endregion

        #region Constructor
        public PageTable()
        {
            InitializeComponent();

            #region Set
            #region sldpnl
            sldpnl.SelectedPage = spList;
            #endregion
            #region grid
            var dg = grid;
            dg.RowHeight = 30;
            dg.Columns.Add(new DvDataGridLabelColumn(dg) { Name = "DeviceName", HeaderText = "장치명", Size = new SizeInfo(DvSizeMode.Percent, 15) });
            dg.Columns.Add(new DvDataGridLabelColumn(dg) { Name = "Time", HeaderText = "설치일", Size = new SizeInfo(DvSizeMode.Percent, 15), FormatString = "yyyy.MM.dd", UseSort = true });
            dg.Columns.Add(new DvDataGridLabelColumn(dg) { Name = "DOW", HeaderText = "요일", Size = new SizeInfo(DvSizeMode.Percent, 10), TextConverter = GetDOW });
            dg.Columns.Add(new DvDataGridLabelColumn(dg) { Name = "Temperature", HeaderText = "온도", Size = new SizeInfo(DvSizeMode.Percent, 10), FormatString = "0.0 ℃" });
            dg.Columns.Add(new DvDataGridLampColumn(dg) { Name = "AlarmT", HeaderText = "온도 알람", Size = new SizeInfo(DvSizeMode.Percent, 10), OnLampColor = Util.FromArgb(180, 0, 0) });
            dg.Columns.Add(new DvDataGridLabelColumn(dg) { Name = "Humidity", HeaderText = "습도", Size = new SizeInfo(DvSizeMode.Percent, 10), FormatString = "0 '%'" });
            dg.Columns.Add(new DvDataGridLampColumn(dg) { Name = "AlarmH", HeaderText = "습도 알람", Size = new SizeInfo(DvSizeMode.Percent, 10), OnLampColor = Util.FromArgb(180, 0, 0) });
            dg.Columns.Add(new DvDataGridButtonColumn(dg) { Name = "Play", HeaderText = "동작", ButtonText = "", IconString = "fa-play", Size = new SizeInfo(DvSizeMode.Percent, 10), Gradient = true });
            dg.Columns.Add(new DvDataGridButtonColumn(dg) { Name = "Stop", HeaderText = "정지", ButtonText = "", IconString = "fa-stop", Size = new SizeInfo(DvSizeMode.Percent, 10), Gradient = true });

            for (int i = 1; i <= 100; i++)
            {
                ls.Add(new GridItem()
                {
                    DeviceName = "DEV" + i,
                    Time = DateTime.Now.Date + TimeSpan.FromDays(i),
                    Humidity = rnd.Next(0, 100),
                    Temperature = rnd.Next(0, 1000) / 10D,
                });
            }

            dg.SetDataSource<GridItem>(ls);

            new Thread((o) =>
            {
                while (true)
                {
                    foreach (var v in ls)
                    {
                        v.Humidity = Convert.ToInt32(MathTool.Constrain(v.Humidity + (rnd.Next() % 2 == 0 ? 1 : -1), 0, 100));
                        v.Temperature = Convert.ToDouble(MathTool.Constrain(v.Temperature + (rnd.Next() % 2 == 0 ? 0.1 : -0.1), 0, 100));
                    }
                    Thread.Sleep(10);
                }
            })
            { IsBackground = true }.Start();
            #endregion
            #endregion

            #region Event
            tool.ItemDragStart += (o, s) => s.Drag = true;
            lblTool.DragDrop += (o, s) =>
            {
                if (s.DragItem is ToolItem)
                {
                    var v = s.DragItem as ToolItem;
                    lblTool.Text = v.Text; 
                }
            };
            #endregion

            dg.SelectionMode = DvDataGridSelectionMode.MULTI;
        }
        #endregion

        #region Override
        protected override void OnUpdate()
        {
            lblList.Text = list.SelectedItems.Count == 1 ? list.SelectedItems.First().Text : "";
            lblTree.Text = tree.SelectedNodes.Count == 1 ? tree.SelectedNodes.First().Text : "";
            base.OnUpdate();
        }
        #endregion

        #region Method
        #region GetDOW
        string GetDOW(object dow)
        {
            var s = "";
            if (dow is DayOfWeek)
            {
                switch (dow)
                {
                    case DayOfWeek.Monday: s = "월"; break;
                    case DayOfWeek.Tuesday: s = "화"; break;
                    case DayOfWeek.Wednesday: s = "수"; break;
                    case DayOfWeek.Thursday: s = "목"; break;
                    case DayOfWeek.Friday: s = "금"; break;
                    case DayOfWeek.Saturday: s = "토"; break;
                    case DayOfWeek.Sunday: s = "일"; break;
                }
            }
            return s;
        }
        #endregion
        #endregion
    }

    #region class : GridItem
    public enum DeviceMode { A, B, C }
    public class GridItem
    {
        public string DeviceName { get; set; }
        public DateTime Time { get; set; }
        public DayOfWeek DOW => Time.DayOfWeek;
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public bool AlarmH => Humidity < 15;
        public bool AlarmT => Temperature > 80;
    }
    #endregion
}
