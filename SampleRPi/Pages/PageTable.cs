using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;

namespace SampleRPi.Pages
{
    public class PageTable : DvPage
    {
        #region Member Variable
        DvSwitchPanel spnl;
        DvSubPage tpListBox, tpToolBox, tpTreeView, tpDataGrid;
        DvButtons menus;
        DvListBox list;
        DvTreeView tree;
        DvToolBox tool;
        DvDataGrid dgrid;
        DvTableLayoutPanel tpnl;
        DvMessageBox MessageBox;

        DvLabel lblDrop;

        Random rnd = new Random();
        #endregion

        #region Constructor
        public PageTable()
        {
            UseMasterPage = true;

            #region MessageBox
            MessageBox = new DvMessageBox();
            #endregion
            #region Tabless
            spnl = new Devinno.Skia.Containers.DvSwitchPanel() { Name = nameof(spnl) };

            tpListBox = new DvSubPage() { Name = nameof(tpListBox) };
            tpToolBox = new DvSubPage() { Name = nameof(tpToolBox) };
            tpTreeView = new DvSubPage() { Name = nameof(tpTreeView) };
            tpDataGrid = new DvSubPage() { Name = nameof(tpDataGrid) };
            spnl.Pages.Add(tpListBox);
            spnl.Pages.Add(tpToolBox);
            spnl.Pages.Add(tpTreeView);
            spnl.Pages.Add(tpDataGrid);
            spnl.SelectedPage = tpListBox;
            #endregion
            #region Menus
            menus = new DvButtons() { Name = nameof(menus), SelectorMode = true };
            menus.Buttons.Add(new ButtonInfo(tpListBox.Name) { Text = "ListBox", Checked = true, IconString = "fa-list" });
            menus.Buttons.Add(new ButtonInfo(tpToolBox.Name) { Text = "ToolBox", IconString = "fa-tools" });
            menus.Buttons.Add(new ButtonInfo(tpTreeView.Name) { Text = "TreeView", IconString = "fa-project-diagram" });
            menus.Buttons.Add(new ButtonInfo(tpDataGrid.Name) { Text = "DataGrid", IconString = "fa-table" });
            #endregion
            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = MainWindow.BaseMargin, Fill = true };
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 100F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 10 });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 36 });

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 50F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 450 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 50F });
            #endregion
            #region list
            list = new DvListBox { Name = nameof(list), Fill = true, BackgroundDraw = true, SelectionMode = ItemSelectionMode.SINGLE, Margin = new Padding(200, 3, 200, 3) };
            for (int i = 1; i <= 50; i++) list.Items.Add(new TextIconItem { Text = "Item " + i });
            #endregion
            #region tool
            tool = new DvToolBox { Name = nameof(tool), Fill = true, Margin = new Padding(200, 3, 200, 3) };
            for (int i = 1; i <= 3; i++)
            {
                var v = new ToolCategoryItem("Category " + i);
                tool.Categories.Add(v);

                for (int j = 1; j <= 10; j++)
                    v.Items.Add(new ToolItem("Item " + i + "." + j));
            }
            tpToolBox.Controls.Add(tool);
            #endregion
            #region tree
            tree = new DvTreeView { Name = nameof(tree), Fill = true, SelectionMode = ItemSelectionMode.SINGLE, Margin = new Padding(200, 3, 200, 3) };

            for (int a = 1; a <= 3; a++)
            {
                var va = new TreeViewNode("Cat " + a);
                tree.Nodes.Add(va);

                for (int b = 1; b <= 4; b++)
                {
                    var vb = new TreeViewNode("Sub " + a + "." + b);
                    va.Nodes.Add(vb);

                    for (int c = 1; c <= 4; c++)
                    {
                        var vc = new TreeViewNode("Item " + a + "." + b + "." + c);
                        vb.Nodes.Add(vc);
                    }
                }
            }

            tpTreeView.Controls.Add(tree);
            #endregion
            #region lblDrop
            lblDrop = new DvLabel { Name = nameof(lblDrop), X = 110, Y = 253, Width = 80, Height = 80, Text = "" };
            tpToolBox.Controls.Add(lblDrop);
            #endregion
            #region dgrid
            dgrid = new DvDataGrid() { Name = nameof(dgrid), Fill = true, Margin = new Padding(20, 3, 20, 3) };
            tpDataGrid.Controls.Add(dgrid);

            #region actMonth
            var actMonth = new Action(() =>
              {
                  var dg = dgrid;
                  dg.SelectionMode = DvDataGridSelectionMode.SELECTOR;
                  dg.ColumnGroups.Clear();
                  dg.Columns.Clear();
                  dg.Rows.Clear();
                  dg.SummaryRows.Clear();

                  dg.TouchMode = true;
                  dg.ScrollMode = ScrollMode.Both;
                  dg.RowHeight = dg.ColumnHeight = Convert.ToInt32(30);
                  dg.ColumnGroups.Add(new DvDataGridColumn(dg) { Name = "G1", HeaderText = "기본사항", Fixed = true });
                  dg.ColumnGroups.Add(new DvDataGridColumn(dg) { Name = "G2", HeaderText = "일일 수집량" });
                  dg.Columns.Add(new DvDataGridColumn(dg) { Name = "Name", GroupName = "G1", HeaderText = "이름", SizeMode = SizeMode.Pixel, Width = Convert.ToInt32(100), Fixed = true, UseFilter = true, CellType = typeof(DvDataGridLabelCell) });
                  dg.Columns.Add(new DvDataGridColumn(dg) { Name = "State", GroupName = "G1", HeaderText = "상태", SizeMode = SizeMode.Pixel, Width = Convert.ToInt32(70), Fixed = true, CellType = typeof(DvDataGridLabelCell) });
                  for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
                      dg.Columns.Add(new DvDataGridColumn(dg) { Name = "Day" + i, GroupName = "G2", HeaderText = i + "일", SizeMode = SizeMode.Pixel, Width = Convert.ToInt32(70), CellType = typeof(DvDataGridLabelCell) });

                  var srow = new DvDataGridSummaryRow(dg);
                  var srow2 = new DvDataGridSummaryRow(dg);
                  srow.Cells.Add(new DvDataGridSummaryLabelCell(dg, srow) { Text = "합계", ColumnIndex = 0, ColumnSpan = 2 });
                  srow.Cells.Add(new DvDataGridSummaryLabelCell(dg, srow) { Text = "", ColumnIndex = 0, ColumnSpan = 1, Visible = false });
                  srow2.Cells.Add(new DvDataGridSummaryLabelCell(dg, srow) { Text = "평균", ColumnIndex = 0, ColumnSpan = 2 });
                  srow2.Cells.Add(new DvDataGridSummaryLabelCell(dg, srow) { Text = "", ColumnIndex = 0, ColumnSpan = 1, Visible = false });
                  for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
                  {
                      srow.Cells.Add(new DvDataGridSummarySumCell(dg, srow) { ColumnIndex = 1 + i, ColumnSpan = 1, Format = "N0" });
                      srow2.Cells.Add(new DvDataGridSummaryAverageCell(dg, srow) { ColumnIndex = 1 + i, ColumnSpan = 1, Format = "N0" });
                  }
                  dg.SummaryRows.Add(srow);
                  dg.SummaryRows.Add(srow2);

                  var Items = new List<GridItem>();
                  for (int i = 0; i <= 100; i++)
                  {
                      var lsv = new List<int>();
                      for (int j = 1; j <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); j++) lsv.Add(rnd.Next(0, 100));
                      Items.Add(new GridItem() { Name = "NM" + i, State = "NORMAL", Days = lsv.ToArray() });
                  }
                  dg.SetDataSource<GridItem>(Items);

              });
            #endregion
            #region actMonitor
            var actMonitor = new Action(() =>
            {
                var pics = new List<SKBitmap>();
                pics.Add(Util.FromBitmap(Path.Combine(PathTool.CurrentPath, "Images", "1.png")));
                pics.Add(Util.FromBitmap(Path.Combine(PathTool.CurrentPath, "Images", "2.png")));
                pics.Add(Util.FromBitmap(Path.Combine(PathTool.CurrentPath, "Images", "3.png")));

                var dg = dgrid;
                dg.TouchMode = true;
                dg.SelectionMode = DvDataGridSelectionMode.SELECTOR;
                dg.ColumnGroups.Clear();
                dg.Columns.Clear();
                dg.Rows.Clear();
                dg.SummaryRows.Clear();

                /*
                dg.RowHeight = 43;
                dg.Columns.Add(new DvDataGridColumn(dg) { Name = "DeviceName", HeaderText = "장치명", SizeMode = SizeMode.Percent, Width = 10M });
                dg.Columns.Add(new DvDataGridImageColumn(dg) { Name = "DeviceImage", HeaderText = "이미지", SizeMode = SizeMode.Percent, Width = 11M });
                dg.Columns.Add(new DvDataGridTextFormatColumn(dg) { Name = "Time", HeaderText = "설치일", SizeMode = SizeMode.Percent, Width = 15M, Format = "yyyy.MM.dd", UseSort = true });
                dg.Columns.Add(new DvDataGridTextConverterColumn(dg) { Name = "DOW", HeaderText = "요일", SizeMode = SizeMode.Percent, Width = 8M, Converter = GetDOW });
                dg.Columns.Add(new DvDataGridTextFormatColumn(dg) { Name = "Temperature", HeaderText = "온도", SizeMode = SizeMode.Percent, Width = 10M, Format = "0.0 ℃" });
                dg.Columns.Add(new DvDataGridLampColumn(dg) { Name = "AlarmT", HeaderText = "온도 알람", SizeMode = SizeMode.Percent, Width = 10M});
                dg.Columns.Add(new DvDataGridTextFormatColumn(dg) { Name = "Humidity", HeaderText = "습도", SizeMode = SizeMode.Percent, Width = 10M, Format = "0 '%'" });
                dg.Columns.Add(new DvDataGridLampColumn(dg) { Name = "AlarmH", HeaderText = "습도 알람", SizeMode = SizeMode.Percent, Width = 10M});
                dg.Columns.Add(new DvDataGridButtonColumn(dg) { Name = "Play", HeaderText = "동작", Text = "", IconString = "fa-play", IconSize = 11, SizeMode = SizeMode.Percent, Width = 8M });
                dg.Columns.Add(new DvDataGridButtonColumn(dg) { Name = "Stop", HeaderText = "정지", Text = "", IconString = "fa-stop", IconSize = 11, SizeMode = SizeMode.Percent, Width = 8M });
                */

                dg.RowHeight = 30;
                dg.Columns.Add(new DvDataGridColumn(dg) { Name = "DeviceName", HeaderText = "장치명", SizeMode = SizeMode.Percent, Width = 15M });
                dg.Columns.Add(new DvDataGridTextFormatColumn(dg) { Name = "Time", HeaderText = "설치일", SizeMode = SizeMode.Percent, Width = 15M, Format = "yyyy.MM.dd", UseSort = true });
                dg.Columns.Add(new DvDataGridTextConverterColumn(dg) { Name = "DOW", HeaderText = "요일", SizeMode = SizeMode.Percent, Width = 10M, Converter = GetDOW });
                dg.Columns.Add(new DvDataGridTextFormatColumn(dg) { Name = "Temperature", HeaderText = "온도", SizeMode = SizeMode.Percent, Width = 10M, Format = "0.0 ℃" });
                dg.Columns.Add(new DvDataGridLampColumn(dg) { Name = "AlarmT", HeaderText = "온도 알람", SizeMode = SizeMode.Percent, Width = 10M });
                dg.Columns.Add(new DvDataGridTextFormatColumn(dg) { Name = "Humidity", HeaderText = "습도", SizeMode = SizeMode.Percent, Width = 10M, Format = "0 '%'" });
                dg.Columns.Add(new DvDataGridLampColumn(dg) { Name = "AlarmH", HeaderText = "습도 알람", SizeMode = SizeMode.Percent, Width = 10M });
                dg.Columns.Add(new DvDataGridButtonColumn(dg) { Name = "Play", HeaderText = "동작", Text = "", IconString = "fa-play", IconSize = 11, SizeMode = SizeMode.Percent, Width = 10M });
                dg.Columns.Add(new DvDataGridButtonColumn(dg) { Name = "Stop", HeaderText = "정지", Text = "", IconString = "fa-stop", IconSize = 11, SizeMode = SizeMode.Percent, Width = 10M });

                var Items = new List<GridItem2>();
                for (int i = 1; i <= 100; i++)
                {
                    Items.Add(new GridItem2()
                    {
                        DeviceName = "DEV" + i,
                        DeviceImage = i % 3 == 0 ? pics[0] : (i % 3 == 1 ? pics[1] : pics[2]),
                        Time = DateTime.Now.Date + TimeSpan.FromDays(i),
                        Humidity = rnd.Next(0, 100),
                        Temperature = rnd.Next(0, 1000) / 10D,
                    });
                }
                dg.SetDataSource<GridItem2>(Items);

                new Thread((o) =>
                {
                    var ls = o as List<GridItem2>;
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
                { IsBackground = true }.Start(Items);
            });
            #endregion
            #region actInput
            var actInput = new Action(() =>
            {
                var dg = dgrid;
                dg.SelectionMode = DvDataGridSelectionMode.SELECTOR;
                dg.ColumnGroups.Clear();
                dg.Columns.Clear();
                dg.Rows.Clear();
                dg.SummaryRows.Clear();

                var ls = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(x => new TextIconItem(GetDOW(x)) { Value = x }).ToList();

                dg.Columns.Add(new DvDataGridColumn(dg) { Name = "Name", HeaderText = "명칭", SizeMode = SizeMode.Percent, Width = 10M });
                dg.Columns.Add(new DvDataGridCheckBoxColumn(dg) { Name = "Used", HeaderText = "사용", SizeMode = SizeMode.Percent, Width = 5M });
                dg.Columns.Add(new DvDataGridComboBoxColumn(dg) { Name = "DOW", HeaderText = "요일", SizeMode = SizeMode.Percent, Width = 10M, Items = ls, MaximumViewCount = 5, ButtonWidth = 30, });
                dg.Columns.Add(new DvDataGridEditTextColumn(dg) { Name = "Message", HeaderText = "메시지", SizeMode = SizeMode.Percent, Width = 12M });
                dg.Columns.Add(new DvDataGridEditNumberColumn<int>(dg) { Name = "Integer", HeaderText = "정수", SizeMode = SizeMode.Percent, Width = 10M, Minimum = 0, Maximum = 100 });
                dg.Columns.Add(new DvDataGridEditNumberColumn<double>(dg) { Name = "Double", HeaderText = "실수", SizeMode = SizeMode.Percent, Width = 10M, Minimum = 0, Maximum = 100 });
                dg.Columns.Add(new DvDataGridEditBoolColumn(dg) { Name = "OnOff", HeaderText = "ON/OFF", SizeMode = SizeMode.Percent, Width = 18M, });
                dg.Columns.Add(new DvDataGridDateTimePickerColumn(dg) { Name = "Time", HeaderText = "날짜", SizeMode = SizeMode.Percent, Width = 13M, PickerMode = DateTimePickerType.Date });
                dg.Columns.Add(new DvDataGridColorPickerColumn(dg) { Name = "Color", HeaderText = "색상", SizeMode = SizeMode.Percent, Width = 12M });
                //18
                var Items = new List<GridItem3>();
                for (int i = 1; i <= 100; i++)
                {
                    Items.Add(new GridItem3()
                    {
                        Name = "아이템" + i,
                        DOW = (DayOfWeek)(i % 7),
                        Time = DateTime.Now.Date,
                        Color = SKColors.Black,
                    });
                }
                dg.SetDataSource<GridItem3>(Items);
            });
            #endregion
            //actMonth();
            //actMonitor();
            actInput();
            #endregion
            #region Controls.Add
            tpListBox.Controls.Add(list);

            tpnl.Controls.Add(menus, 1, 2);
            tpnl.Controls.Add(spnl, 0, 0, 3, 1);

            Controls.Add(tpnl);
            #endregion

            #region Event
            menus.ButtonClick += (o, s) => spnl.SelectedPageName = s.Button.Name;
            list.ItemDoubleClicked += (o, s) => MessageBox.ShowMessageBoxOk("더블클릭", s.Item.Text, (result) => { });

            tool.ItemDown += (o, s) => s.Drag = true;
            lblDrop.DragDrop += (o, s) =>
            {
                if (s.DragItem is TextIconItem)
                {
                    var v = s.DragItem as TextIconItem;
                    lblDrop.Text = v.Text;
                }
            };
            #endregion
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
    public class GridItem
    {
        public string Name { get; set; }
        public string State { get; set; }

        public int Day1 { get => Days[0]; }
        public int Day2 { get => Days[1]; }
        public int Day3 { get => Days[2]; }
        public int Day4 { get => Days[3]; }
        public int Day5 { get => Days[4]; }
        public int Day6 { get => Days[5]; }
        public int Day7 { get => Days[6]; }
        public int Day8 { get => Days[7]; }
        public int Day9 { get => Days[8]; }
        public int Day10 { get => Days[9]; }
        public int Day11 { get => Days[10]; }
        public int Day12 { get => Days[11]; }
        public int Day13 { get => Days[12]; }
        public int Day14 { get => Days[13]; }
        public int Day15 { get => Days[14]; }
        public int Day16 { get => Days[15]; }
        public int Day17 { get => Days[16]; }
        public int Day18 { get => Days[17]; }
        public int Day19 { get => Days[18]; }
        public int Day20 { get => Days[19]; }
        public int Day21 { get => Days[20]; }
        public int Day22 { get => Days[21]; }
        public int Day23 { get => Days[22]; }
        public int Day24 { get => Days[23]; }
        public int Day25 { get => Days[24]; }
        public int Day26 { get => Days[25]; }
        public int Day27 { get => Days[26]; }
        public int Day28 { get => Days[27]; }
        public int Day29 { get => Days[28]; }
        public int Day30 { get => Days[29]; }
        public int Day31 { get => Days[30]; }

        public int[] Days { get; set; } = new int[31];
    }
    #endregion
    #region class : GridItem2
    public enum DeviceMode { A, B, C }
    public class GridItem2
    {
        public string DeviceName { get; set; }
        public SKBitmap DeviceImage { get; set; }
        public DateTime Time { get; set; }
        public DayOfWeek DOW => Time.DayOfWeek;
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public bool AlarmH => Humidity < 15;
        public bool AlarmT => Temperature > 80;
    }
    #endregion
    #region class : GridItem3
    public class GridItem3
    {
        public string Name { get; set; }
        public bool Used { get; set; }
        public DayOfWeek DOW { get; set; }
        public string Message { get; set; }
        public int Integer { get; set; }
        public double Double { get; set; }
        public bool OnOff { get; set; }
        public DateTime Time { get; set; }
        public SKColor Color { get; set; }
    }
    #endregion
}
