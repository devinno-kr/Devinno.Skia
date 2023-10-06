using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;
using System;
using Devinno.Skia.Design;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.ES30;
using GLES = OpenTK.Graphics.ES30.GL;
using GL = OpenTK.Graphics.OpenGL.GL;
using GetPNameES = OpenTK.Graphics.ES30.GetPName;
using GetPName = OpenTK.Graphics.OpenGL.GetPName;
using RenderbufferTargetES = OpenTK.Graphics.ES30.RenderbufferTarget;
using RenderbufferTarget = OpenTK.Graphics.OpenGL.RenderbufferTarget;
using RenderbufferParameterNameES = OpenTK.Graphics.ES30.RenderbufferParameterName;
using RenderbufferParameterName = OpenTK.Graphics.OpenGL.RenderbufferParameterName;
using Devinno.Skia.Utils;
using System.Text;
using Devinno.Skia.Tools;

namespace Devinno.Skia.OpenTK
{
    public class DvManualWindow : Base.ManualWindow
    {
        #region Properties
        public int Width => Size.X;
        public int Height => Size.Y;

        public DvDesign Design { get; private set; }
        #endregion

        #region Member Variable
        private GRGlInterface gli;
        private GRContext ctx;
        private GRBackendRenderTarget target;
        private DateTime dcTime;
        #endregion

        #region Constructor
        public DvManualWindow(int width, int height)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                WindowBorder = WindowBorder.Hidden,
                Size = new Vector2i(width, height),
                API = (Environment.OSVersion.Platform == PlatformID.Unix ? ContextAPI.OpenGLES : ContextAPI.OpenGL),
                APIVersion = (Environment.OSVersion.Platform == PlatformID.Unix ? new Version(3, 1) : new Version(3, 2))
            })
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix) this.WindowState = WindowState.Fullscreen;

            Design = new DvDesign() { Width = width, Height = height };
        }

        public DvManualWindow(WindowState state)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                WindowState = state,
                WindowBorder = WindowBorder.Hidden,
                API = (Environment.OSVersion.Platform == PlatformID.Unix ? ContextAPI.OpenGLES : ContextAPI.OpenGL),
                APIVersion = (Environment.OSVersion.Platform == PlatformID.Unix ? new Version(3, 1) : new Version(3, 2))
            })
        {

            Design = new DvDesign() { Width = Bounds.Size.X, Height = Bounds.Size.Y };
        }

        public DvManualWindow(int width, int height, double UpdateFreq)
            : base(new GameWindowSettings()
            {
                UpdateFrequency = UpdateFreq,
            },
                new NativeWindowSettings()
                {
                    WindowBorder = WindowBorder.Hidden,
                    Size = new Vector2i(width, height),
                    API = (Environment.OSVersion.Platform == PlatformID.Unix ? ContextAPI.OpenGLES : ContextAPI.OpenGL),
                    APIVersion = (Environment.OSVersion.Platform == PlatformID.Unix ? new Version(3, 1) : new Version(3, 2))
                })
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix) this.WindowState = WindowState.Fullscreen;

            Design = new DvDesign() { Width = width, Height = height };
        }
        #endregion

        #region Override
        #region OnLoad
        protected override void OnLoad()
        {
            var bnd = new GLFWBindingsContext();
            GL.LoadBindings(bnd);

            if (Environment.OSVersion.Platform == PlatformID.Unix)
                gli = GRGlInterface.CreateGles(new GRGlGetProcedureAddressDelegate((name) => bnd.GetProcAddress(name)));
            else
                gli = GRGlInterface.CreateOpenGl(new GRGlGetProcedureAddressDelegate((name) => bnd.GetProcAddress(name)));

            ctx = GRContext.CreateGl(gli);
            target = CreateRenderTarget();

            base.OnLoad();
        }
        #endregion
        #region OnUnload
        protected override void OnUnload()
        {
            Design?.Unload();

            base.OnUnload();
            gli?.Dispose(); gli = null;
            target?.Dispose(); target = null;
            ctx?.Dispose(); ctx = null;
        }
        #endregion
        #region OnRenderFrame
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);


            if (ctx != null && target != null)
            {
                if (Design != null)
                {
                    Design.Width = Width;
                    Design.Height = Height;
                }

                using (var surface = SKSurface.Create(ctx, target, SKColorType.Rgba8888))
                {
                    if (surface != null)
                    {
                        var canvas = surface.Canvas;

                        Design?.Draw(canvas);

                    }
                }

                ctx.Flush();

                SwapBuffers();
            }
        }
        #endregion
        #region OnUpdateFrame
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (ctx != null && target != null)
            {
                Design?.Update();
            }
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Design?.MouseDown(Convert.ToInt32(MousePosition.X), Convert.ToInt32(MousePosition.Y));
             
            base.OnMouseDown(e);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Design?.MouseUp(Convert.ToInt32(MousePosition.X), Convert.ToInt32(MousePosition.Y));

            if ((DateTime.Now - dcTime).TotalMilliseconds < 300)
            {
                Design?.MouseDoubleClick(Convert.ToInt32(MousePosition.X), Convert.ToInt32(MousePosition.Y));
            }

            dcTime = DateTime.Now;
           
            base.OnMouseUp(e);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Design?.MouseMove(Convert.ToInt32(MousePosition.X), Convert.ToInt32(MousePosition.Y));
           
            base.OnMouseMove(e);
        }
        #endregion
        #endregion

        #region Method
        #region CreateRenderTarget
        GRBackendRenderTarget CreateRenderTarget()
        {
            int stencil = 8;
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                GLES.GetInteger(GetPNameES.FramebufferBinding, out int framebuffer);
                GLES.GetInteger(GetPNameES.StencilBits, out stencil);
                GLES.GetInteger(GetPNameES.Samples, out int samples);
                stencil = 8;
                GLES.GetRenderbufferParameter(RenderbufferTargetES.Renderbuffer, RenderbufferParameterNameES.RenderbufferWidth, out int bufferWidth);
                GLES.GetRenderbufferParameter(RenderbufferTargetES.Renderbuffer, RenderbufferParameterNameES.RenderbufferHeight, out int bufferHeight);
            }
            else
            {
                GL.GetInteger(GetPName.FramebufferBinding, out int framebuffer);
                GL.GetInteger(GetPName.StencilBits, out stencil);
                GL.GetInteger(GetPName.Samples, out int samples);
                stencil = 8;
                GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth, out int bufferWidth);
                GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight, out int bufferHeight);

            }
            return new GRBackendRenderTarget(Width, Height, 0, stencil, new GRGlFramebufferInfo(0, SKColorType.Rgba8888.ToGlSizedFormat()));
        }
        #endregion
        #endregion
    }
}
