using NekoPlayer.Containers;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Wpf.ItemsControlViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

namespace NekoPlayer.Wpf.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для CacheManagerDialog.xaml
    /// </summary>
    public partial class CacheManagerDialog : UserControl, INotifyPropertyChanged
    {
        public CacheManagerDialog()
        {
            InitializeComponent();
            Menus = new CompositeCollection();
            Menus.Add(new BooleanControl("DeleteCacheFileAtNextStart", "Delete cache on next session", "Delete cache to free space, but after that you will lost all online track informations and online album covers.\n\nYou can switch it back if you changed mind before closes application."));
        }
        public CompositeCollection Menus { get; private set; }

        private string m_MessageText;
        public string MessageText { get => m_MessageText; private set { m_MessageText = value; OnPropertyChanged(); } }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(nameof(Menus));
            Task.Run(() =>
            {
                try
                {
                    MessageText = "Calculating...";
                    ProcessedSegment data = CacheManager.GetStatusSegmentData();
                    //long DBSize = CacheManager.GetCacheFileBytes();
                    StatementCache.Dispatcher.Invoke(() =>
                    {
                        MessageText = "Please wait...";
                        //StatusBar.SetSegmentData(data);
                        //StatusBar.SetMaximum(DBSize);
                        //StatusBar.SetVisibility(true);
                        StatementCache.ItemsSource = data.SegmentParts;
                        StatementCache.Visibility = Visibility.Visible;
                        IndeterminateBar.Dispatcher.Invoke(() => IndeterminateBar.Visibility = Visibility.Collapsed);
                    });
                }
                catch(Exception e)
                {
                    ExceptMessage.PopupExcept(e);
                }
            });
        }
        public void OnDialogClosed()
        {
            StatementCache.Dispatcher.Invoke(() =>
            {
                StatementCache.ItemsSource = null;
            });
        }
    }
}
