using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.InteropServices;
using Devinno.Skia.Tools;

namespace SampleRPi
{
    public class Program
    {
		#region Interop
		const int SW_HIDE = 0;
		const int SW_SHOW = 1;

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion

        public static MainWindow MainWindow { get; private set; }

		static void Main(string[] args)
        {
			/*
			if(Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
				var handle = GetConsoleWindow();
				ShowWindow(handle, SW_HIDE);
			}
			*/

			for (decimal v = -2M; v <= 2M; v += 0.5M)
				Console.WriteLine($"{v} : {(v < 0M ? MathTool.Map((double)v, -2, 0, 0.5, 1) : MathTool.Map((double)v, 0, 2, 1, 2))}");


            using (var view = new MainWindow())
            {
				MainWindow = view;
				view.VSync = VSyncMode.On;
                view.CenterWindow();
                view.Run();
				MainWindow = null;
            }
        }
    }

	class A<T> where T : struct
	{
		public T? Value { get; set; }

	}

}
