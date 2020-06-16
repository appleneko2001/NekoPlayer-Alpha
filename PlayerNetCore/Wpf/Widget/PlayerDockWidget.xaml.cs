using Appleneko2001;
using DialogAdapters;
using NekoPlayer.Core.Engine;
using NekoPlayer.Globalization;
using NekoPlayer.Wpf.ModelViews;
using PlayerNetCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NekoPlayer.Wpf.Widget
{
    /// <summary>
    /// Dock control panel, contains buttons for control host Play/Pause, next track, volumes or etc.
    /// Context model from <see cref="GlobalViewModel"/>
    /// </summary>
    public partial class PlayerDockWidget : UserControl
    {
        #region Variables and properties
        public bool IsCursorFocused { get; private set; }
        private GlobalViewModel PlayerContext;
        private BassEngine PlayerHost;
        private bool DraggingSlider;
        private bool ThumbButtonUpToResume = false;
        #endregion
        #region Constructor
        /// <summary>
        /// Initialize widget for use.
        /// </summary>
        public PlayerDockWidget()
        {
            InitializeComponent();
            PlayerHost = App.BassHost;
            PlayerContext = GlobalViewModel.GetInstance();
        }
        #endregion
        #region Public methods
        public void ThumbMoveStart()
        {
            if (!DraggingSlider)
            {
                ThumbButtonUpToResume = PlayerHost.IsPlaying;
                PlayerHost.Pause();
            }
            DraggingSlider = true;
        }
        public void ThumbMoveEnd()
        {
            if (DraggingSlider)
            {
                DraggingSlider = false;
                PlayerHost.SetPosition(PlayerContext.CurrentPosition);
                if (ThumbButtonUpToResume)
                    PlayerHost.Resume();
            }
        }
        #endregion
        #region Private methods (for events binding and logics)
        private Thumb GetSliderThumb(Slider obj)
        {
            var track = obj.Template.FindName("PART_Track", obj) as Track;
            return track?.Thumb;
        }

        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            Thumb thumb = GetSliderThumb((Slider)sender);
            thumb.MouseEnter += Thumb_MouseEnter;
            thumb.MouseLeave += delegate (object s, MouseEventArgs _e) { ThumbMoveEnd(); };
            thumb.PreviewMouseLeftButtonUp += delegate (object sender, MouseButtonEventArgs e) { ThumbMoveEnd(); };
            thumb.DragStarted += delegate(object s, DragStartedEventArgs _e) { ThumbMoveStart(); };
            thumb.DragCompleted += delegate (object s, DragCompletedEventArgs _e) { ThumbMoveEnd(); };
            thumb.Unloaded += delegate (object s, RoutedEventArgs _e) {
                thumb.MouseEnter -= Thumb_MouseEnter;
                thumb.MouseLeave -= delegate (object s, MouseEventArgs _e) { ThumbMoveEnd(); };
                thumb.PreviewMouseLeftButtonUp -= delegate (object sender, MouseButtonEventArgs e) { ThumbMoveEnd(); };
                thumb.DragStarted -= delegate (object s, DragStartedEventArgs _e) { ThumbMoveStart(); };
                thumb.DragCompleted -= delegate (object s, DragCompletedEventArgs _e) { ThumbMoveEnd(); };
            };
        }

        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.MouseDevice.Captured == null)
            {
                var thumb = sender as Thumb;
                ThumbButtonUpToResume = PlayerHost.IsPlaying;
                PlayerHost.Pause();
                DraggingSlider = true;
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);
                args.RoutedEvent = MouseLeftButtonDownEvent;
                thumb.RaiseEvent(args);
                //PlayerHost.SetPosition(PlayerContext.CurrentPosition);
            }
        }

        private void Root_MouseEnter(object sender, MouseEventArgs e)
        {
            IsCursorFocused = true;
        }

        private void Root_MouseLeave(object sender, MouseEventArgs e)
        {
            IsCursorFocused = false;
        }

        private void PlaylistItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = FindName("PlaylistListView") as ListView;
            Player.PlayPlayableOnCurrentPlaylist(lv.SelectedIndex);
        }
        private void PlaylistListView_UpdateColumn(object sender)
        {
            var view = (sender as ListView);
            var gridView = view.View as GridView;
            AutoResizeGridViewColumns(gridView, view.ActualWidth);
        }

        private void PlaylistListView_Initialized(object sender, EventArgs e)
        {
            var lv = sender as ListView;
            lv.Loaded += (s, a) => PlaylistListView_UpdateColumn(s);
            lv.TargetUpdated += (s, a) => PlaylistListView_UpdateColumn(s);
            lv.Unloaded += (s, a) =>
            {
                lv.Loaded -= (s, a) => PlaylistListView_UpdateColumn(s);
                lv.TargetUpdated -= (s, a) => PlaylistListView_UpdateColumn(s);
            };
        }
        #endregion
        #region Control autosize columns (for playlist listview)
        static void AutoResizeGridViewColumns(GridView view, double width = 0)
        {
            if (view == null || view.Columns.Count < 1) return;
            List<GridViewColumn> columns = new List<GridViewColumn>();
            double delta = 40;         // Simulates column auto sizing
            foreach (var column in view.Columns)
            {
                // Forcing change
                var fill = column.Header as TextBlock;
                if (fill != null)
                {
                    var tag = fill.Tag as string;
                    if (tag.Contains("fill", StringComparison.InvariantCultureIgnoreCase))
                        columns.Add(column);
                }
                else
                {
                    if (!double.IsNaN(column.Width))
                        delta += column.Width;
                }
            }
            foreach (var c in columns)
            {
                double w = width - delta;
                if (w >= 0) c.Width = w; else c.Width = 0;
            }
        }
        #endregion
    }
}
