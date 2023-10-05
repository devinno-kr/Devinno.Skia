using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvInputBox : DvWindow
    {
        #region Properties
        public int ColumnCount { get; set; } = 1;

        public int ItemHeight { get; set; } = 40;
        public int ItemTitleWidth { get; set; } = 80;
        public int ItemValueWidth { get; set; } = 150;

        public int? DialogWidth { get; set; } = null;
        public int? DialogHeight { get; set; } = null;
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvTableLayoutPanel tpnl;
        DvGridLayoutPanel gpnl;

        DvButton btnOK, btnCancel;

        bool bOK = false;
        #endregion

        #region Constructor
        public DvInputBox()
        {
            IconString = "fa-pen-to-square";
            Width = 300;
            Height = 400;

            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = new Padding(0), Fill = true };
            #endregion
            #region gpnl
            gpnl = new DvGridLayoutPanel() { Name = nameof(gpnl), Margin = new Padding(10, TitleHeight + 10, 10, 10), Fill = true };
            gpnl.Rows.Add(new DvGridRow(DvSizeMode.Percent, 100)); 
            gpnl.Rows.Add(new DvGridRow(DvSizeMode.Pixel, 46));
            gpnl.Rows[0].Columns.Add(new SizeInfo(DvSizeMode.Percent, 100));
            gpnl.Rows[1].Columns.Add(new SizeInfo(DvSizeMode.Percent, 100));
            gpnl.Rows[1].Columns.Add(new SizeInfo(DvSizeMode.Pixel, 90));
            gpnl.Rows[1].Columns.Add(new SizeInfo(DvSizeMode.Pixel, 90));
            #endregion
            #region New
            btnOK = new DvButton { Name = nameof(btnOK), Text = "확인", Gradient = true };
            btnCancel = new DvButton { Name = nameof(btnCancel), Text = "취소", Gradient = true };
            #endregion
            #region Controls.Add
            gpnl.Controls.Add(tpnl, 0, 0);
            gpnl.Controls.Add(btnOK, 1, 1);
            gpnl.Controls.Add(btnCancel, 2, 1);
            Controls.Add(gpnl);
            #endregion
            #region btn[OK/Cancel].MouseClick
            btnCancel.MouseClick += (o, s) => { bOK = false; Hide(); };
            btnOK.MouseClick += (o, s) => { bOK = true; Hide(); };
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
        #region show
        void show(string Title, Action actLayout, Action actOK)
        {
            this.Title = Title;

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
        #region ShowInputBox
        public void ShowInputBox<T>(string Title, T value, Dictionary<string, InputBoxInfo> infos, Action<T> result) where T : class
        {
            show(Title,
                () =>
                {
                    #region var
                    var ps = typeof(T).GetProperties();
                    var props = ps.Where(x => CheckProp(x, (infos != null && infos.ContainsKey(x.Name) ? infos[x.Name] : null))).ToList();
                    var RowCount = Convert.ToInt32(Math.Ceiling((double)props.Count / (double)ColumnCount));
                    var csz = 100F / ColumnCount;
                    var rsz = 100F / RowCount;
                    #endregion
                    #region Size
                    var w = 10 + (ColumnCount * (ItemTitleWidth + ItemValueWidth + 6)) + 10;
                    var h = TitleHeight + 10 + (RowCount * (ItemHeight + 6)) + 10 + 46 + 10;

                    Width = DialogWidth ?? w;
                    Height = DialogHeight ?? h;
                    #endregion
                    #region Layout
                    tpnl.Rows.Clear();
                    tpnl.Columns.Clear();

                    for (int i = 0; i < ColumnCount; i++) tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, csz));
                    for (int i = 0; i < RowCount; i++) tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, rsz));
                    #endregion
                    #region Props
                    tpnl.Controls.Clear();
                    foreach(var v in props)
                    {
                        #region var
                        var p = (infos != null && infos.ContainsKey(v.Name) ? infos[v.Name] : null);

                        var title = p?.Title ?? v.Name;
                        var count = tpnl.Controls.Count;
                        var col = count % ColumnCount;
                        var row = count / ColumnCount;
                        var min = p?.Minimum;
                        var max = p?.Maximum;
                        var format = p?.FormatString;
                        #endregion

                        #region Items
                        if (p != null && p.Items != null && p.Items.Count > 0)
                        {
                            #region Selector
                            var c = new DvValueInputSelector
                            {
                                Name = v.Name,
                                Title = title,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                            };

                            tpnl.Controls.Add(c, col, row);
                            c.Items.AddRange(p.Items);
                            if (value != null)
                            {
                                var val = v.GetValue((object)value);
                                var itm = c.Items.Where(x => val != null && val.Equals(x.Value)).FirstOrDefault();
                                if (itm != null) c.SelectedIndex = c.Items.IndexOf(itm);
                            }
                            #endregion
                        }
                        else if (v.PropertyType == typeof(sbyte))
                        {
                            #region byte
                            var c = new DvValueInputNumber<sbyte>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToSByte(min.Value) : null,
                                Maximum = max != null ? Convert.ToSByte(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (sbyte)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(short))
                        {
                            #region short
                            var c = new DvValueInputNumber<short>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToInt16(min.Value) : null,
                                Maximum = max != null ? Convert.ToInt16(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (short)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(int))
                        {
                            #region int
                            var c = new DvValueInputNumber<int>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToInt32(min.Value) : null,
                                Maximum = max != null ? Convert.ToInt32(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (int)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(long))
                        {
                            #region long
                            var c = new DvValueInputNumber<long>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToInt64(min.Value) : null,
                                Maximum = max != null ? Convert.ToInt64(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (long)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(byte))
                        {
                            #region byte
                            var c = new DvValueInputNumber<byte>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToByte(min.Value) : null,
                                Maximum = max != null ? Convert.ToByte(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (byte)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(ushort))
                        {
                            #region ushort
                            var c = new DvValueInputNumber<ushort>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToUInt16(min.Value) : null,
                                Maximum = max != null ? Convert.ToUInt16(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (ushort)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(uint))
                        {
                            #region uint
                            var c = new DvValueInputNumber<uint>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToUInt32(min.Value) : null,
                                Maximum = max != null ? Convert.ToUInt32(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (uint)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(ulong))
                        {
                            #region ulong
                            var c = new DvValueInputNumber<ulong>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToUInt64(min.Value) : null,
                                Maximum = max != null ? Convert.ToUInt64(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (ulong)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(float))
                        {
                            #region float
                            var c = new DvValueInputNumber<float>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToSingle(min.Value) : null,
                                Maximum = max != null ? Convert.ToSingle(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (float)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(double))
                        {
                            #region double
                            var c = new DvValueInputNumber<double>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToDouble(min.Value) : null,
                                Maximum = max != null ? Convert.ToDouble(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (double)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(decimal))
                        {
                            #region decimal
                            var c = new DvValueInputNumber<decimal>
                            {
                                Name = v.Name,
                                Title = title,
                                Minimum = min != null ? Convert.ToDecimal(min.Value) : null,
                                Maximum = max != null ? Convert.ToDecimal(max.Value) : null,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                                FormatString = format
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (decimal)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(string))
                        {
                            #region string
                            var c = new DvValueInputText
                            {
                                Name = v.Name,
                                Title = title,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (string)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType == typeof(bool))
                        {
                            #region bool
                            var c = new DvValueInputBool
                            {
                                Name = v.Name,
                                Title = title,
                                TitleAreaSize = ItemTitleWidth,
                                OnText = p?.OnText ?? "ON",
                                OffText = p?.OffText ?? "OFF",
                                Tag = new InputBoxTag() { prop = v, attr = p },
                            };
                            tpnl.Controls.Add(c, col, row);
                            if (value != null) c.Value = (bool)v.GetValue((object)value);
                            #endregion
                        }
                        else if (v.PropertyType.IsEnum)
                        {
                            #region enum
                            var c = new DvValueInputSelector
                            {
                                Name = v.Name,
                                Title = title,
                                TitleAreaSize = ItemTitleWidth,
                                Tag = new InputBoxTag() { prop = v, attr = p },
                            };
                            tpnl.Controls.Add(c, col, row);
                            c.Items.AddRange(Enum.GetValues(v.PropertyType).Cast<object>().Select(x => new SelectorItem() { Text = x.ToString(), Value = x }));

                            if (value != null)
                            {
                                var val = v.GetValue((object)value);
                                var itm = c.Items.Where(x => val != null && val.Equals(x.Value)).FirstOrDefault();
                                if (itm != null) c.SelectedIndex = c.Items.IndexOf(itm);
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                },
                () =>
                {
                    if (bOK)
                    {
                        var v = (T)Activator.CreateInstance(typeof(T));

                        #region Value
                        foreach (var c in tpnl.Controls.Values)
                        {
                            var tag = c.Tag as InputBoxTag;
                            if (tag != null)
                            {
                                var p = tag.prop;
                                var info = tag.attr;

                                if (c is DvValueInputNumber<byte>) p.SetValue(v, ((DvValueInputNumber<byte>)c).Value);
                                else if (c is DvValueInputNumber<ushort>) p.SetValue(v, ((DvValueInputNumber<ushort>)c).Value);
                                else if (c is DvValueInputNumber<uint>) p.SetValue(v, ((DvValueInputNumber<uint>)c).Value);
                                else if (c is DvValueInputNumber<ulong>) p.SetValue(v, ((DvValueInputNumber<ulong>)c).Value);
                                else if (c is DvValueInputNumber<sbyte>) p.SetValue(v, ((DvValueInputNumber<sbyte>)c).Value);
                                else if (c is DvValueInputNumber<short>) p.SetValue(v, ((DvValueInputNumber<short>)c).Value);
                                else if (c is DvValueInputNumber<int>) p.SetValue(v, ((DvValueInputNumber<int>)c).Value);
                                else if (c is DvValueInputNumber<long>) p.SetValue(v, ((DvValueInputNumber<long>)c).Value);
                                else if (c is DvValueInputNumber<float>) p.SetValue(v, ((DvValueInputNumber<float>)c).Value);
                                else if (c is DvValueInputNumber<double>) p.SetValue(v, ((DvValueInputNumber<double>)c).Value);
                                else if (c is DvValueInputNumber<decimal>) p.SetValue(v, ((DvValueInputNumber<decimal>)c).Value);
                                else if (c is DvValueInputText) p.SetValue(v, ((DvValueInputText)c).Value);
                                else if (c is DvValueInputBool) p.SetValue(v, ((DvValueInputBool)c).Value);
                                else if (c is DvValueInputSelector)
                                {
                                    var vc = c as DvValueInputSelector;
                                    if (vc.SelectedIndex >= 0 && vc.SelectedIndex < vc.Items.Count)
                                    {
                                        var vt = vc.Items[vc.SelectedIndex].Value;
                                        p.SetValue(v, vc.Items[vc.SelectedIndex].Value);
                                    }
                                }
                            }
                        }
                        #endregion

                        result(v);
                    }
                    else result(default(T));
                });

        }
        #endregion

        #region CheckProp
        bool CheckProp(PropertyInfo prop, InputBoxInfo attr)
        {
            bool ret = false;
            var p = attr;
           
            if (p != null && p.Items != null && p.Items.Count > 0) ret = true;
            if (prop.PropertyType == typeof(sbyte)) ret = true;
            else if (prop.PropertyType == typeof(short)) ret = true;
            else if (prop.PropertyType == typeof(int)) ret = true;
            else if (prop.PropertyType == typeof(long)) ret = true;
            else if (prop.PropertyType == typeof(byte)) ret = true;
            else if (prop.PropertyType == typeof(ushort)) ret = true;
            else if (prop.PropertyType == typeof(uint)) ret = true;
            else if (prop.PropertyType == typeof(ulong)) ret = true;
            else if (prop.PropertyType == typeof(float)) ret = true;
            else if (prop.PropertyType == typeof(double)) ret = true;
            else if (prop.PropertyType == typeof(decimal)) ret = true;
            else if (prop.PropertyType == typeof(string)) ret = true;
            else if (prop.PropertyType == typeof(bool)) ret = true;
            else if (prop.PropertyType.IsEnum) ret = true;

            return ret && prop.CanWrite && prop.CanRead && !Attribute.IsDefined(prop, typeof(InputBoxIgnoreAttribute));
        }
        #endregion
        #endregion
    }

    #region attr
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class InputBoxIgnoreAttribute : Attribute
    {
    }
    #endregion
    #region class : InputBoxInfo
    public class InputBoxInfo
    {
        public decimal? Minimum { get; set; } = null;
        public decimal? Maximum { get; set; } = null;

        public string Title { get; set; }
        public string FormatString { get; set; }
        public string OnText { get; set; }
        public string OffText { get; set; }
        public List<SelectorItem> Items { get; set; }
    }
    #endregion
    #region class : InputBoxTag
    internal class InputBoxTag
    {
        public PropertyInfo prop { get; set; }
        public InputBoxInfo attr { get; set; }
    }
    #endregion
}
