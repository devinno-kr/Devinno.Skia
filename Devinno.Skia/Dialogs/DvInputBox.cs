using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public int ItemWidth { get; set; } = 174;
        public int ItemHeight { get; set; } = 30;
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
            IconString = "fa-edit";
            Width = 300;
            Height = 400;

            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = new Padding(10, 10, 10, 10), Fill = true };
            #endregion
            #region gpnl
            gpnl = new DvGridLayoutPanel() { Name = nameof(gpnl), Margin = new Padding(0) };
            gpnl.Rows.Add(new DvGridRow { Mode = SizeMode.Percent, Size = 100 });
            gpnl.Rows[0].Columns.Add(new DvGridColumn { Mode = SizeMode.Percent, Size = 100 });
            gpnl.Rows[0].Columns.Add(new DvGridColumn { Mode = SizeMode.Pixel, Size = 90 });
            gpnl.Rows[0].Columns.Add(new DvGridColumn { Mode = SizeMode.Pixel, Size = 90 });
            #endregion
            #region New
            btnOK = new DvButton { Name = nameof(btnOK), Text = "확인" };
            btnCancel = new DvButton { Name = nameof(btnCancel), Text = "취소" };
            #endregion
            #region Controls.Add
            Controls.Add(tpnl);
            gpnl.Controls.Add(btnOK, 1, 0);
            gpnl.Controls.Add(btnCancel, 2, 0);
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
        void show(string Title, Action act1, Action act2)
        {
            this.Title = Title;

            act1();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) => { Thread.Sleep(DvDesign.HIDE_TIME); act2(); });

            });
        }
        #endregion
        #region ShowInputBox
        public void ShowInputBox<T>(string Title, T value, Dictionary<string, InputBoxInfo> infos, Action<T> result) where T : class
        {
            show(Title, () =>
            {
                var ps = typeof(T).GetProperties();
                var props = ps.Where(x => CheckProp(x, infos)).ToList();
                var RowCount = Convert.ToInt32(Math.Ceiling((double)props.Count / (double)ColumnCount));
                var csz = 100F / ColumnCount;
                var rsz = 100F / RowCount;

                this.Width = Math.Max(200, 10 + (ColumnCount * (ItemWidth + 6)) + 10);
                this.Height = Math.Max(106, TitleHeight + 10 + (RowCount * (ItemHeight + 6)) + 10 + 36 + 10);

                #region Layout
                tpnl.Rows.Clear();
                tpnl.Columns.Clear();

                for (int i = 0; i < ColumnCount; i++) tpnl.Columns.Add(new DvTableColumn { Mode = SizeMode.Percent, Size = csz });
                for (int i = 0; i < RowCount; i++) tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Percent, Size = rsz });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 10 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 36 });
                #endregion
                #region New
                tpnl.Controls.Clear();

                var nstr = props.Select(x => infos.ContainsKey(x.Name) ? infos[x.Name].Title : x.Name).OrderByDescending(x => x.Length).FirstOrDefault();
                var nsz = Math.Max(60, Convert.ToInt32(Util.MeasureText(nstr, FontName, 12, DvFontStyle.Normal).Width + 20));

                foreach (var v in props)
                {
                    var p = infos.ContainsKey(v.Name) ? infos[v.Name] : null;

                    var title = p != null ? p.Title : v.Name;
                    var count = tpnl.Controls.Count;
                    var col = count % ColumnCount;
                    var row = count / ColumnCount;
                    var min = p != null ? p.Minimum : null;
                    var max = p != null ? p.Maximum : null;
                    var unit = p != null ? p.Unit : null;

                    if (p != null && p.Items != null && p.Items.Count > 0)
                    {
                        var c = new DvSelectorValueInput { Name = v.Name, Title = title, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        c.Items.AddRange(p.Items);
                        if (value != null)
                        {
                            var val = v.GetValue((object)value);
                            var itm = c.Items.Where(x => val != null && val.Equals(x.Value)).FirstOrDefault();
                            if (itm != null) c.SelectedIndex = c.Items.IndexOf(itm);
                        }
                    }
                    else if (v.PropertyType == typeof(sbyte))
                    {
                        var c = new DvNumberValueInput<sbyte> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToSByte(min.Value) : null, Maximum = max != null ? Convert.ToSByte(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (sbyte)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(short))
                    {
                        var c = new DvNumberValueInput<short> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToInt16(min.Value) : null, Maximum = max != null ? Convert.ToInt16(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (short)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(int))
                    {
                        var c = new DvNumberValueInput<int> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToInt32(min.Value) : null, Maximum = max != null ? Convert.ToInt32(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (int)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(long))
                    {
                        var c = new DvNumberValueInput<long> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToInt64(min.Value) : null, Maximum = max != null ? Convert.ToInt64(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (long)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(byte))
                    {
                        var c = new DvNumberValueInput<byte> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToByte(min.Value) : null, Maximum = max != null ? Convert.ToByte(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (byte)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(ushort))
                    {
                        var c = new DvNumberValueInput<ushort> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToUInt16(min.Value) : null, Maximum = max != null ? Convert.ToUInt16(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (ushort)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(uint))
                    {
                        var c = new DvNumberValueInput<uint> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToUInt32(min.Value) : null, Maximum = max != null ? Convert.ToUInt32(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (uint)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(ulong))
                    {
                        var c = new DvNumberValueInput<ulong> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToUInt64(min.Value) : null, Maximum = max != null ? Convert.ToUInt64(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (ulong)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(float))
                    {
                        var c = new DvNumberValueInput<float> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToSingle(min.Value) : null, Maximum = max != null ? Convert.ToSingle(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (float)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(double))
                    {
                        var c = new DvNumberValueInput<double> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToDouble(min.Value) : null, Maximum = max != null ? Convert.ToDouble(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (double)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(decimal))
                    {
                        var c = new DvNumberValueInput<decimal> { Name = v.Name, Title = title, Minimum = min != null ? Convert.ToDecimal(min.Value) : null, Maximum = max != null ? Convert.ToDecimal(max.Value) : null, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (decimal)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(string))
                    {
                        var c = new DvTextValueInput { Name = v.Name, Title = title, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (string)v.GetValue((object)value);
                    }
                    else if (v.PropertyType == typeof(bool))
                    {
                        var c = new DvBoolValueInput { Name = v.Name, Title = title, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        if (value != null) c.Value = (bool)v.GetValue((object)value);
                    }
                    else if (v.PropertyType.IsEnum)
                    {
                        var c = new DvSelectorValueInput { Name = v.Name, Title = title, TitleWidth = nsz, Tag = new InputBoxTag() { p = v, info = p }, Unit = unit };
                        tpnl.Controls.Add(c, col, row);
                        c.Items.AddRange(Enum.GetValues(v.PropertyType).Cast<object>().Select(x => new TextIconItem() { Text = x.ToString(), Value = x }));

                        if (value != null)
                        {
                            var val = v.GetValue((object)value);
                            var itm = c.Items.Where(x => val != null && val.Equals(x.Value)).FirstOrDefault();
                            if (itm != null) c.SelectedIndex = c.Items.IndexOf(itm);
                        }
                    }
                }
                tpnl.Controls.Add(gpnl, 0, RowCount + 1, ColumnCount, 1);
                #endregion

            }, () =>
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
                            var p = tag.p;
                            var info = tag.info;

                            if (c is DvNumberValueInput<byte>) p.SetValue(v, ((DvNumberValueInput<byte>)c).Value);
                            else if (c is DvNumberValueInput<ushort>) p.SetValue(v, ((DvNumberValueInput<ushort>)c).Value);
                            else if (c is DvNumberValueInput<uint>) p.SetValue(v, ((DvNumberValueInput<uint>)c).Value);
                            else if (c is DvNumberValueInput<ulong>) p.SetValue(v, ((DvNumberValueInput<ulong>)c).Value);
                            else if (c is DvNumberValueInput<sbyte>) p.SetValue(v, ((DvNumberValueInput<sbyte>)c).Value);
                            else if (c is DvNumberValueInput<short>) p.SetValue(v, ((DvNumberValueInput<short>)c).Value);
                            else if (c is DvNumberValueInput<int>) p.SetValue(v, ((DvNumberValueInput<int>)c).Value);
                            else if (c is DvNumberValueInput<long>) p.SetValue(v, ((DvNumberValueInput<long>)c).Value);
                            else if (c is DvNumberValueInput<float>) p.SetValue(v, ((DvNumberValueInput<float>)c).Value);
                            else if (c is DvNumberValueInput<double>) p.SetValue(v, ((DvNumberValueInput<double>)c).Value);
                            else if (c is DvNumberValueInput<decimal>) p.SetValue(v, ((DvNumberValueInput<decimal>)c).Value);
                            else if (c is DvTextValueInput) p.SetValue(v, ((DvTextValueInput)c).Value);
                            else if (c is DvBoolValueInput) p.SetValue(v, ((DvBoolValueInput)c).Value);
                            else if (c is DvSelectorValueInput)
                            {
                                var vc = c as DvSelectorValueInput;
                                var vt = vc.Items[vc.SelectedIndex].Value;
                                p.SetValue(v, vc.Items[vc.SelectedIndex].Value);
                            }
                        }
                    }
                    #endregion

                    result(v);
                }
                else result(default(T));
            });

        }

        public void ShowInputBox<T>(string Title, Action<T> result) where T : class => ShowInputBox<T>(Title, null, null, result);
        public void ShowInputBox<T>(string Title, T value, Action<T> result) where T : class => ShowInputBox<T>(Title, value, null, result);
        public void ShowInputBox<T>(string Title, Dictionary<string, InputBoxInfo> infos, Action<T> result) where T : class => ShowInputBox<T>(Title, null, infos, result);
        #endregion
        #region ShowString
        public void ShowString(string Title, string value, Action<string> result)
        {
            show(Title, () =>
            {
                this.Width = Math.Max(200, 10 + (ColumnCount * (ItemWidth + 6)) + 10);
                this.Height = Math.Max(106, TitleHeight + 10 + (ItemHeight + 6) + 10 + 36 + 10);

                #region Layout
                tpnl.Rows.Clear();
                tpnl.Columns.Clear();

                tpnl.Columns.Add(new DvTableColumn { Mode = SizeMode.Percent, Size = 100F });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Percent, Size = 100F });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 10 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 36 });
                #endregion
                #region New
                tpnl.Controls.Clear();

                var c = new DvTextInput { Name = "value" };
                tpnl.Controls.Add(c, 0, 0);
                tpnl.Controls.Add(gpnl, 0, 1 + 1, 1, 1);

                if (value != null) c.Value = value;
                #endregion

            }, () =>
            {
                if (bOK)
                {
                    var ret = ((DvTextInput)tpnl.Controls["value"]).Value;
                    result(ret);
                }
                else
                {
                    result(null);
                }
            });

        }
        #endregion
        #region CheckProp
        bool CheckProp(PropertyInfo prop, Dictionary<string, InputBoxInfo> infos)
        {
            bool ret = false;
            var p = infos != null ? (infos.ContainsKey(prop.Name) ? infos[prop.Name] : null) : null;

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


    #region attr : InputBoxIgnore
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class InputBoxIgnoreAttribute : Attribute { }
    #endregion
    #region class : InputBoxInfo
    public class InputBoxInfo
    {
        public decimal? Minimum { get; set; } = null;
        public decimal? Maximum { get; set; } = null;

        public string Title { get; set; }
        public string Unit { get; set; }
        public List<TextIconItem> Items { get; set; }
    }
    #endregion
    #region class : InputBoxTag
    internal class InputBoxTag
    {
        public PropertyInfo p { get; set; }
        public InputBoxInfo info { get; set; }
    }
    #endregion
}
