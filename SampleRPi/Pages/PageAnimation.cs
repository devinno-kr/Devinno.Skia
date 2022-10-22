using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleRPi.Pages
{
    public class PageAnimation : DvPage
    {
        #region Member Variable
        DvSwitchPanel spnl;
        DvSubPage tpMovie, tpGif;
        DvAnimate gif;
        DvMovie movie;
        DvButtons menus;
        DvTableLayoutPanel tpnl;
        #endregion

        #region Constructor
        public PageAnimation()
        {
            UseMasterPage = true;

            #region Tabless
            spnl = new Devinno.Skia.Containers.DvSwitchPanel() { Name = nameof(spnl) };
            tpGif = new DvSubPage() { Name = nameof(tpGif) };
            tpMovie = new DvSubPage() { Name = nameof(tpMovie) };
            spnl.Pages.Add(tpGif);
            spnl.Pages.Add(tpMovie);
            spnl.SelectedPage = tpGif;
            #endregion
            #region Menus
            menus = new DvButtons() { Name = nameof(menus), SelectorMode = true };
            menus.Buttons.Add(new ButtonInfo(tpGif.Name) { Text = "Animate", IconString = "fa-film", Checked = true });
            menus.Buttons.Add(new ButtonInfo(tpMovie.Name) { Text = "Video", IconString = "fa-video" });
           
            #endregion
            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = MainWindow.BaseMargin, Fill = true };
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 100F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 10 });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 36 });

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 50F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 200 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 50F });
            #endregion
            #region gif
            gif = new DvAnimate { Name = nameof(gif), Fill = true, Margin = new Padding(200, 3, 200, 3) };

            tpGif.Controls.Add(gif);
            #endregion
            #region movie
            movie = new DvMovie { Name = nameof(movie), Fill = true, Margin = new Padding(120, 3, 120, 3) };

            tpMovie.Controls.Add(movie);
            #endregion
            #region Controls.Add
            tpnl.Controls.Add(menus, 1, 2);
            tpnl.Controls.Add(spnl, 0, 0, 3, 1);

            Controls.Add(tpnl);
            #endregion
            #region Event
            menus.ButtonClick += (o, s) => spnl.SelectedPageName = s.Button.Name;
            #endregion

            #region Gif
            //var path = Path.Combine(PathTool.CurrentPath, "Images", "ani.gif");
            //gif.LoadGIF(path);
            var path = Path.Combine(PathTool.CurrentPath, "Images", "ani.gif");
            gif.Load(Directory.GetFiles(Path.Combine(PathTool.CurrentPath, "Images", "ani")).OrderBy(x => Convert.ToInt32(Path.GetFileNameWithoutExtension(x).Substring(3))).ToArray());
            gif.MouseClick += (o, s) => gif.OnOff = !gif.OnOff;
            #endregion
            #region Movie
            var path2 = Path.Combine(PathTool.CurrentPath, "Images", "sample.avi");
            //var path2 = "rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mp4";
            movie.Repeat = true;
            
            movie.Url = path2;
            movie.MouseClick += (o, s) =>
            {
                if (movie.IsStart) movie.Stop();
                else movie.Start();
            };
            #endregion
        }
        #endregion

        #region Method
        #endregion
    }
}
