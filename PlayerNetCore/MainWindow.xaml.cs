using Hardcodet.Wpf.TaskbarNotification;
using MaterialDesignThemes.Wpf;
using NekoPlayer;
using NekoPlayer.Core.Engine;
using NekoPlayer.Globalization;
using NekoPlayer.Pages;
using NekoPlayer.Wpf.Dialogs;
using NekoPlayer.Wpf.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PlayerNetCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel context;
        private ChangelogDialog changelogDialog;
        private bool DoNotShutdownApp;
        public MainWindow()
        {
            InitializeComponent();
            context = new MainWindowModel();
            
            DataContext = context;
        }

        private void LDButton_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LDButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var tag = e.Parameter;//(sender as Button).Tag;
            if (tag is string)
            {
                context.JumpPage((string)tag);
                LeftDrawerSwitch.Dispatcher.Invoke(() => LeftDrawerSwitch.IsChecked = false);
            }
        }
        internal void DonotShutdownApp(bool value) => DoNotShutdownApp = value;
        /// <summary>
        /// Part procedures of event if main window is completely closed (No hide!)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (SettingsManager.GetValue<bool>("StopWhenNoWindow", false))
            {
                if(!DoNotShutdownApp)
                    App.Current.Shutdown();
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // We hide it if parameter "StopWhenNoWindow is false", otherwize we just do close program, no any handled etc.
            if(!SettingsManager.GetValue<bool>("StopWhenNoWindow", false))
            {
                //e.Cancel = true;
                RequestSave();
                //Dispatcher.Invoke(() => Close());
                if (!DoNotShutdownApp)
                    App.ShowTrayIcon();

            }
        }

        private void RequestSave()
        {
            GlobalViewModel.GetInstance().RequestSaveSettings();
            GlobalViewModel.GetInstance().RecentPlaylist.RequestSaveChanges();
            BassEngine.RequestSaveSettings();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (changelogDialog is null)
                changelogDialog = new ChangelogDialog();
            await DialogHost.Show(changelogDialog, openedEventHandler: null, closingEventHandler: (obj, args) => NekoPlayer.VersionInfos.VersionInfo.DoOptimize()).ConfigureAwait(false);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (context.GetTag() == "Home")
                {
                    var obj = context.GetUserControlObject() as WindowMainPage;
                    if (obj != null)
                    {
                        if(obj.IsMouseOverPlayerDock())
                        {
                            Player.OnPlayPauseKeyboardCommandExecuted();
                        }
                    }
                }
            }
        }
    }
}
