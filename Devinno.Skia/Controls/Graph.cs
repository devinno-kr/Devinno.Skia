using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    #region enum : BarGraphMode 
    public enum BarGraphMode { STACK, LIST }
    #endregion

    #region class : GV
    class GV
    {
        public string Name { get; set; }
        public SKColor Color { get; set; }
        public Dictionary<string, double> Values
        {
            get
            {
                var ret = new Dictionary<string, double>();
                foreach (var vk in Props.Keys) ret.Add(vk, Convert.ToDouble(Props[vk].GetValue(Data)));
                return ret;
            }
        }

        internal Dictionary<string, PropertyInfo> Props { get; set; }
        internal GraphData Data { get; set; }
    }
    #endregion

    #region class : GraphSeries
    public class GraphSeries
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public SKColor SeriesColor { get; set; }
    }
    #endregion
    #region class : GraphSeries2
    public class GraphSeries2 : GraphSeries
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public bool Visible { get; set; } = true;
    }
    #endregion

    #region abstract class : GraphData
    public abstract class GraphData
    {
        public abstract string Name { get; set; }
        public SKColor Color { get; set; }
    }
    #endregion
    #region abstract class : TimeGraphData
    public abstract class TimeGraphData
    {
        public abstract DateTime Time { get; set; }
    }
    #endregion

    #region class : TGV
    class TGV
    {
        public DateTime Time { get; set; }
        public Dictionary<string, double> Values { get; } = new Dictionary<string, double>();
    }
    #endregion
    #region class : LGV
    class LGV
    {
        public SKPoint Position { get; set; }
        public double Value { get; set; }
    }
    #endregion
    #region class : CGV
    class CGV
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public SKColor Color { get; set; }
    }
    #endregion

}
