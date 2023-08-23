using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Tools
{
    public class ValueTool
    {
        #region Text
        public static string Text<T>(T Value, string FormatString)
        {
            var ret = "";

            var tp = typeof(T);
            if (tp == typeof(sbyte)) ret = FormatString != null ? ((sbyte)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(short)) ret = FormatString != null ? ((short)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(int)) ret = FormatString != null ? ((int)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(long)) ret = FormatString != null ? ((long)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(byte)) ret = FormatString != null ? ((byte)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(ushort)) ret = FormatString != null ? ((ushort)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(uint)) ret = FormatString != null ? ((uint)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(ulong)) ret = FormatString != null ? ((ulong)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(float)) ret = FormatString != null ? ((float)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(double)) ret = FormatString != null ? ((double)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(decimal)) ret = FormatString != null ? ((decimal)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(string)) ret = (string)(object)Value;
            else if (tp == typeof(bool)) ret = ((bool)(object)Value).ToString();
            else if (tp == typeof(DateTime)) ret = FormatString != null ? ((DateTime)(object)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(TimeSpan)) ret = FormatString != null ? ((TimeSpan)(object)Value).ToString(FormatString) : Value.ToString();
            else ret = Value.ToString();

            return ret;
        }

        public static string Text (object Value, string FormatString)
        {
            var ret = "";

            var tp = Value.GetType();
            if (tp == typeof(sbyte)) ret = FormatString != null ? ((sbyte)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(short)) ret = FormatString != null ? ((short)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(int)) ret = FormatString != null ? ((int)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(long)) ret = FormatString != null ? ((long)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(byte)) ret = FormatString != null ? ((byte)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(ushort)) ret = FormatString != null ? ((ushort)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(uint)) ret = FormatString != null ? ((uint)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(ulong)) ret = FormatString != null ? ((ulong)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(float)) ret = FormatString != null ? ((float)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(double)) ret = FormatString != null ? ((double)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(decimal)) ret = FormatString != null ? ((decimal)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(string)) ret = (string)Value;
            else if (tp == typeof(bool)) ret = ((bool)Value).ToString();
            else if (tp == typeof(DateTime)) ret = FormatString != null ? ((DateTime)Value).ToString(FormatString) : Value.ToString();
            else if (tp == typeof(TimeSpan)) ret = FormatString != null ? ((TimeSpan)Value).ToString(FormatString) : Value.ToString();
            else ret = Value.ToString();

            return ret;
        }
        #endregion
    }
}
