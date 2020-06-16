using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using LinqToDB.Tools;
using MaterialDesignThemes.Wpf.Transitions;
using NekoPlayer.Core;
using NekoPlayer.Core.Interfaces;
using NekoPlayer.Globalization;
using NekoPlayer.Wpf.Interfaces;
using NekoPlayer.Wpf.ModelViews;
using NekoPlayer.Wpf.Widget;

namespace NekoPlayer.Pages
{
    public partial class WindowMainPage : UserControl, IGetHeader, IGetPageContent, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyValueChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public IPlaylist SelectedPlaylist { get; private set; }
        private readonly GlobalViewModel GlobalViewModel;
        private readonly NormalHeaderWidget header;
        private readonly NormalHeaderModel headerContext;
        private Binding PlaylistNameBinding;
        public WindowMainPage()
        {
            headerContext = new NormalHeaderModel(null, LanguageManager.RequestNode("home.header"), () => BackBrowse_Executed(PageController, null), e => PageController.SelectedIndex > 0);
            header = new NormalHeaderWidget(headerContext);
            GlobalViewModel = GlobalViewModel.GetInstance();
             InitializeComponent();
            PageController.DataContext = this;
        }

        public object PageContent => Content;
        public ViewModelBase GetHeaderContext()
        {
            return headerContext;
        }
        public object GetHeaderObject()
        {
            return header;
        }

        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            Button control = sender as Button;
            TextBlock name = control.Template.FindName("NameTextBlock", control) as TextBlock;
            if (control.Tag is string)
            {
                var query = GlobalViewModel.RequestPlaylist((string)control.Tag);
                SelectedPlaylist = query;
                OnPropertyValueChanged(nameof(SelectedPlaylist));
                PageController.Dispatcher.Invoke(() => PageController.SelectedIndex = 1);
                var queryBinding = name.GetBindingExpression(TextBlock.TextProperty);
                PlaylistNameBinding = new Binding
                {
                    Source = queryBinding.ResolvedSource,
                    Path = queryBinding.ParentBinding.Path,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                header.SetBindingText(PlaylistNameBinding);
            }
        }
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void PlaylistButton_Drop(object sender, DragEventArgs e)
        {
            var dc = (sender as Control).DataContext;
            if (!(dc is IPlaylist))
                return;
            IPlaylist playlist = dc as IPlaylist;
            if(e.Effects.HasFlag(DragDropEffects.Move) || e.Effects.HasFlag(DragDropEffects.Copy))
            {
                var data = (string[])e.Data.GetData(DataFormats.FileDrop);
                Dispatcher thread = Dispatcher.CurrentDispatcher;
                Task.Run(() =>
                {
                    foreach (var file in data)
                    {
                        if (Playable.TryCreate(file, out Playable playable))
                        {
                            thread.Invoke(() => playlist.AddPlayable(playable));
                        }
                    }
                    playlist.RequestSaveChanges();
                });
            }
        }

        private void PlaylistItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = FindName("PlaylistListView") as ListView;
            GlobalViewModel.SetCurrentPlaylist(SelectedPlaylist);
            Player.PlayPlayableOnCurrentPlaylist(lv.SelectedIndex);
        }
        public bool IsMouseOverPlayerDock() => PlayerDock.IsMouseOver;

        private void BackBrowse_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (sender as Transitioner).SelectedIndex > 0;
        }

        private void BackBrowse_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var t = sender as Transitioner;
            if(t.SelectedIndex > 0)
                t.SelectedIndex -= 1;
            if (t.SelectedIndex == 0)
            {
                header.SetBindingText(null);
                PlaylistNameBinding = null;
            }
        }

        private void PlaylistListView_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            var view = (sender as ListView);
            var gridView = view.View as GridView;
            AutoResizeGridViewColumns(gridView, view.ActualWidth);
        }
        static void AutoResizeGridViewColumns(GridView view, double width = 0)
        {
            if (view == null || view.Columns.Count < 1) return;
            List<GridViewColumn> columns = new List<GridViewColumn>();
            double delta = 20;         // Simulates column auto sizing
            foreach (var column in view.Columns)
            {
                // Forcing change
                if (column.Header is TextBlock fill)
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

        private void PlaylistListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var view = (sender as ListView);
            var gridView = view.View as GridView;
            AutoResizeGridViewColumns(gridView, view.ActualWidth);
        }
    }
}
