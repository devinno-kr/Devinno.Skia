using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils2 = OpenTK.Core.Utils;

namespace Devinno.Skia.OpenTK.Base
{
    public class ManualWindow : NativeWindow
    {
        public event Action Load;
        public event Action Unload;

        public event Action<FrameEventArgs> UpdateFrame;
        public event Action<FrameEventArgs> RenderFrame;

        public int Interval { get; set; } = 0;

        public ManualWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(nativeWindowSettings)
        {
        }

        #region Win32 Function for timing
        /*
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

        [DllImport("kernel32")]
        private static extern IntPtr GetCurrentThread();

        [DllImport("winmm")]
        private static extern uint timeBeginPeriod(uint uPeriod);

        [DllImport("winmm")]
        private static extern uint timeEndPeriod(uint uPeriod);
        */
        #endregion

        public virtual unsafe void Run()
        {
            Context?.MakeCurrent();

            OnLoad();

            OnResize(new ResizeEventArgs(ClientSize));

            while (GLFW.WindowShouldClose(WindowPtr) == false)
            {
                NewInputFrame();

                ProcessWindowEvents(IsEventDriven);

                OnUpdateFrame(new FrameEventArgs());
                OnRenderFrame(new FrameEventArgs());

                if (API != ContextAPI.NoAPI)
                {
                    if (VSync == VSyncMode.Adaptive)
                    {
                        GLFW.SwapInterval(0);
                    }
                }
                Thread.Sleep(Interval);
            }

            OnUnload();
        }
         
        public virtual void SwapBuffers()
        {
            if (Context == null)
            {
                throw new InvalidOperationException("Cannot use SwapBuffers when running with ContextAPI.NoAPI.");
            }

            Context.SwapBuffers();
        }

        public override void Close() => base.Close();
        protected virtual void OnLoad() => Load?.Invoke();
        protected virtual void OnUnload() => Unload?.Invoke();

        protected virtual void OnUpdateFrame(FrameEventArgs args) => UpdateFrame?.Invoke(args);
        protected virtual void OnRenderFrame(FrameEventArgs args) => RenderFrame?.Invoke(args);
    }
}

