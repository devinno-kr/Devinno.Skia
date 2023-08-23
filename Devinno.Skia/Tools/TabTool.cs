using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Tools
{
    internal class TabTool
    {
        #region TabCur
        internal static SKRect TabCur(SKRect rt, DvPosition tabPosition)
        {
            var ret = new SKRect();
            var nc = 7;
            switch (tabPosition)
            {
                case DvPosition.Left: ret = Util.FromRect(rt.Right - nc, rt.Top, nc, rt.Height); break;
                case DvPosition.Top: ret = Util.FromRect(rt.Left, rt.Bottom - nc, rt.Width, nc); break;
                case DvPosition.Right: ret = Util.FromRect(rt.Left, rt.Top, nc, rt.Height); break;
                case DvPosition.Bottom: ret = Util.FromRect(rt.Left, rt.Top, rt.Width, nc); break;
            }
            return ret;
        }
        #endregion
        #region TabText
        internal static SKRect TabText(SKRect rt, DvPosition tabPosition)
        {
            var ret = new SKRect();
            var nc = 5;
            switch (tabPosition)
            {
                case DvPosition.Left: ret = Util.FromRect(rt.Left, rt.Top, rt.Width - nc, rt.Height); break;
                case DvPosition.Top: ret = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height - nc); break;
                case DvPosition.Right: ret = Util.FromRect(rt.Left + nc, rt.Top, rt.Width - nc, rt.Height); break;
                case DvPosition.Bottom: ret = Util.FromRect(rt.Left, rt.Top + nc, rt.Width, rt.Height - nc); break;
            }
            return ret;
        }
        #endregion

        #region AniPage
        internal static void AniPage(DvSubPageCollection TabPages, DvSubPage nowSelTab, DvSubPage prevSelTab,
            DvPosition TabPosition,
            SKRect rtContent, SKRect rtPage,
            Animation ani,
            Action<SKRect, SKRect, byte, byte> act)
        {
            var si = TabPages.Values.ToList().IndexOf(nowSelTab);
            if (si != -1)
            {
                #region var
                var rtP = new SKRect();
                var rtN = new SKRect();
                var aP = Convert.ToByte(ani.Value(AnimationAccel.DCL, 255, 0));
                var aN = Convert.ToByte(ani.Value(AnimationAccel.DCL, 0, 255));
                #endregion

                #region Page
                if (TabPosition == DvPosition.Top || TabPosition == DvPosition.Bottom)
                {
                    #region SlideH
                    var _rtP = Util.FromRect(rtPage);
                    var _rtN = Util.FromRect(rtPage);

                    if (ani.Variable == "Prev")
                    {
                        _rtP.Offset(rtPage.Width, 0);
                        _rtN.Offset(-rtPage.Width, 0);
                    }
                    else if (ani.Variable == "Next")
                    {
                        _rtP.Offset(-rtPage.Width, 0);
                        _rtN.Offset(rtPage.Width, 0);
                    }

                    rtP = ani.Value(AnimationAccel.DCL, rtPage, _rtP);
                    rtN = ani.Value(AnimationAccel.DCL, _rtN, rtPage);
                    #endregion
                }
                else if (TabPosition == DvPosition.Left || TabPosition == DvPosition.Right)
                {
                    #region SlideV
                    var _rtP = Util.FromRect(rtPage);
                    var _rtN = Util.FromRect(rtPage);

                    if (ani.Variable == "Prev")
                    {
                        _rtP.Offset(0, rtPage.Height);
                        _rtN.Offset(0, -rtPage.Height);
                    }
                    else if (ani.Variable == "Next")
                    {
                        _rtP.Offset(0, -rtPage.Height);
                        _rtN.Offset(0, rtPage.Height);
                    }

                    rtP = ani.Value(AnimationAccel.DCL, rtPage, _rtP);
                    rtN = ani.Value(AnimationAccel.DCL, _rtN, rtPage);
                    #endregion
                }
                #endregion

                act(rtP, rtN, aP, aN);
            }
        }
        #endregion
        #region AniTabCursor
        internal static void AniTabCursor(DvSubPageCollection TabPages, DvSubPage nowSelTab, DvSubPage prevSelTab,
            DvPosition tabPosition,
            Dictionary<string, SKRect> dicTab,
            Animation ani,
            Action<SKRect, SKRect> act)
        {
            #region var
            var rtTab = new SKRect();
            var rtTabCur = new SKRect();
            #endregion

            #region Tab
            if (nowSelTab != null && prevSelTab != null && dicTab.ContainsKey(nowSelTab.Name) && dicTab.ContainsKey(prevSelTab.Name))
            {
                var rtp = TabTool.TabCur(dicTab[prevSelTab.Name], tabPosition);
                var rtn = TabTool.TabCur(dicTab[nowSelTab.Name], tabPosition);
            
                rtTab = ani.Value(AnimationAccel.DCL, dicTab[prevSelTab.Name], dicTab[nowSelTab.Name]);
                rtTabCur = ani.Value(AnimationAccel.DCL, rtp, rtn);
            }
            #endregion

            act(rtTab, rtTabCur);
        }
        #endregion
        #region AniTabText
        internal static void AniTabText(DvSubPageCollection TabPages, DvSubPage nowSelTab, DvSubPage prevSelTab,
            DvSubPage tab,
            Animation ani,
            Action<byte> act)
        {
            #region var
            byte a = Convert.ToByte(nowSelTab == tab ? 255 : 60);
            #endregion

            #region Tab
            if (tab == nowSelTab)
            {
                a = Convert.ToByte(ani.Value(AnimationAccel.Linear, 60, 255));
            }

            if (tab == prevSelTab)
            {
                a = Convert.ToByte(ani.Value(AnimationAccel.Linear, 255, 60));
            }
            #endregion

            act(a);
        }
        #endregion
    }
}
