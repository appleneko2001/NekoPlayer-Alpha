using Hardcodet.Wpf.TaskbarNotification;
using MaterialDesignThemes.Wpf;
using NekoPlayer;
using NekoPlayer.Core.Engine;
using NekoPlayer.Containers;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Globalization;
using NekoPlayer.Networking;
using NekoPlayer.VersionInfos;
using NekoPlayer.Wpf.ConverterKinds;
using NekoPlayer.Wpf.ModelViews;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Runtime;

namespace PlayerNetCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    #pragma warning disable CA1001, CA1060
    public partial class App : Application
    {
        #region Commands
        public static RelayCommand ShutdownCommand = new RelayCommand((obj) => Instance.Shutdown());
        public static RelayCommand ShowWindowCommand = new RelayCommand((obj) =>
        {
            PrepareAndShowWindow();
            var tray = (obj as TaskbarIcon);
            if(tray != null)
                tray.Visibility = Visibility.Collapsed;
        });

        //private static CommandBinding PlayButtonTrigger = new CommandBinding(MediaCommands.TogglePlayPause, (s, e) => Player.PlayPauseCommand.Execute(null), (s, e) => e.CanExecute = Player.PlayPauseCommand.CanExecute(null));
        //private static CommandBinding NextButtonTrigger = new CommandBinding(MediaCommands.NextTrack, (s, e) => Player.NextCommand.Execute(true), (s, e) => e.CanExecute = Player.NextCommand.CanExecute(true));
        //private static CommandBinding PreviousButtonTrigger = new CommandBinding(MediaCommands.PreviousTrack, (s, e) => Player.PreviousCommand.Execute(true), (s, e) => e.CanExecute = Player.PreviousCommand.CanExecute(true));
        private static int playButtonId, nextButtonId, prevButtonId;
        #endregion
        #region Variables and properties
        public static string CurrentVersion => VersionInfo.CurrentVersion.DisplayVersion;
        public static bool isAMD64Architecture { get; private set; }
        public static OSPlatform CurrentOSPlatform { get; private set; }
        public static BassEngine BassHost { get; private set; }
        public static MainWindow MainWindowClass { get; private set; }
        public const string DefaultAlbumImageUri = @"pack://application:,,,/NekoPlayer.NetCore;component/Wpf/Resources/Album_Default.png";
        public const string RecentImageUri = @"pack://application:,,,/NekoPlayer.NetCore;component/Wpf/Resources/Recent_Playlist.png";
        public const string IconUri = @"pack://application:,,,/NekoPlayer.NetCore;component/Wpf/Resources/Icon.ico";
        private static Mutex OneInstanceMutexLock;
        private TaskbarIcon TrayIcon;
        private string AppPath;
        private static App Instance;
        private bool RequestRestart = false;
        #endregion
        private void OnStartup(object sender, StartupEventArgs e)
        {
            #region Detect platform and architecture
            isAMD64Architecture = Environment.Is64BitProcess;
            var OS = Environment.OSVersion.Platform;
            if (OS == PlatformID.Win32NT)
            {
                CurrentOSPlatform = OSPlatform.Windows;
            }
            else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    CurrentOSPlatform = OSPlatform.Linux;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    CurrentOSPlatform = OSPlatform.OSX;
                else
                    CurrentOSPlatform = OSPlatform.FreeBSD;
            }
            #endregion
            #region Startup arguments
            foreach (var arg in e.Args)
            {
                if(arg.StartsWith("/OUTPUTLANGPACK", StringComparison.InvariantCultureIgnoreCase))
                {
                    string path = Path.Combine(Environment.CurrentDirectory, "FallbackLanguage.json");
                    var fallbackLangPack = new LanguagePackDataModel(new FallbackLanguage());
                    var result = Newtonsoft.Json.JsonConvert.SerializeObject(fallbackLangPack, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(path, result);
                    Process.GetCurrentProcess().Kill();
                }
                else if (arg.StartsWith("/VERBOSE", StringComparison.InvariantCultureIgnoreCase))
                {
                    AttachConsole(-1);
                    AllocConsole();
                    ExceptMessage.SetDebugMode(true);
                    ExceptMessage.PrintConsole("Show full exception is allowed on current session.");
                }
                else if (arg.StartsWith("/HELP", StringComparison.InvariantCultureIgnoreCase))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("Availables command: ");
                    builder.AppendLine("/outputlangpack   Export fallback language pack for translate or other purposes.");
                    builder.AppendLine("/verbose   For debug purposes.");
                    
                    builder.AppendLine("\nAny command are allowed uppercases or lowercases.");
                    MessageBox.Show(builder.ToString());
                    Process.GetCurrentProcess().Kill();
                }
            }

            #endregion
            #region Only one instance detection (Mutex method)
            OneInstanceMutexLock = new Mutex(false, "NekoPlayer_PlayerNetCore_Mutex_OnlyOneInstanceMutex", out bool instanceMutexCreated);
            if (!instanceMutexCreated)
            {
                ExceptMessage.PopupExcept("Only one instance allowed. If your NekoPlayer instance has no responces, just kill it and start again.");
                var p = Process.GetCurrentProcess();
                p.Kill();
            }
            #endregion
            Variants.LoadPackIconVariants();
            #region Choose container type by OS environment
            if (CurrentOSPlatform == OSPlatform.Windows)
            {
                IDataContainer dataContainer = new JsonDataContainer(SettingsManager.UserAppdataPath);
                SettingsManager.Initialize(new RegistryManager(), dataContainer);
            }
            else
            {
                //Still work in progress, so there's just null.
                //And program will stops working.
                SettingsManager.Initialize(null, null);
            }
            #endregion
            LanguageManager.Initialize();
            LanguageManager.LoadSettings();
            if (SettingsManager.GetValue<bool>("DeleteCacheFileAtNextStart"))
            {
                CacheManager.DeleteCacheFile();
                SettingsManager.SetValue("DeleteCacheFileAtNextStart", false);
            }
            CacheManager.Initialize();
            string OutputDevid = SettingsManager.GetValue<string>("SelectedOutputDevice", null);
            IBassDevice selectedDevice = BassDevice.GetBassDevice(OutputDevid);
            GlobalViewModel.Initialize();
            MusicNetApiManager.Initialize();
            HotKeyBinding.InitializeComponent();
            GCSettings.LatencyMode = GCLatencyMode.Batch;
            BassHost = new BassEngine(selectedDevice);
            NekoPlayer.Player.BindBassEvents(BassHost);
            RegisterGlobalKeyBinding();
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            ApplyDarkTheme(SettingsManager.GetValue<bool>("UseDarkTheme", false));
            Instance = this;
            AppPath = Process.GetCurrentProcess().MainModule.FileName;

            InitTrayIcon();
            PrepareAndShowWindow();
        }
        public static void ShowTrayIcon() => Instance.TrayIcon.Dispatcher.Invoke(() => Instance.TrayIcon.Visibility = Visibility.Visible);
        private void InitTrayIcon()
        {
            TrayIcon = new TaskbarIcon
            {
                Visibility = Visibility.Collapsed,
                IconSource = new BitmapImage(new Uri(IconUri)),
                ContextMenu = FindResource("MenuOnTrayIcon") as ContextMenu
            };
            TrayIcon.TrayMouseDoubleClick += (obj, arg) => ShowWindowCommand.Execute(TrayIcon);
            TrayIcon.TrayToolTip = FindResource("ToolTipTrayBlob") as UIElement;
        }
        private static void PrepareAndShowWindow()
        {
            if (MainWindowClass is null)
            {
                MainWindowClass = new MainWindow();
                MainWindowClass.Closed += MainWindowClass_Closed;
                MainWindowClass.Show();
            }
        }
        private static void MainWindowClass_Closed(object sender, EventArgs e)
        {
            if (MainWindowClass != null)
            {
                MainWindowClass.Closed -= MainWindowClass_Closed;
            }
            MainWindowClass = null;
        }
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptMessage.PrintConsole(e.Exception);
            ExceptMessage.PopupExcept(e.Exception, false);
            e.Handled = true;
        }
        #region Themes control
        /// <summary>
        /// Apply a dark theme, make background and everything light thing to dark.
        /// </summary>
        /// <param name="isOn">Apply True value if you want to use dark theme, otherwize we toggle it back to white.</param>
        public static void ApplyDarkTheme(bool isOn)
        {
            ModifyTheme(theme => theme.SetBaseTheme(isOn ? Theme.Dark : Theme.Light));
        }
        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
        #endregion
        #region Key bindings and unbinds
        /// <summary>
        /// Binding media function buttons and it will work if your keyboard contains some media buttons, and it will triggered when you press it.
        /// This method should be called on startup.
        /// </summary>
        private static void RegisterGlobalKeyBinding()
        {
            //CommandManager.RegisterClassCommandBinding(typeof(object), PlayButtonTrigger);
            //CommandManager.RegisterClassCommandBinding(typeof(object), NextButtonTrigger);
            //CommandManager.RegisterClassCommandBinding(typeof(object), PreviousButtonTrigger);
            //Those commands will not works when application in background. Should at least one
            //Control element for handling. So there I will use RegisterHotKey() instead
            //Low-level key binding are greater than sh**ty High-level key binding sometimes
            var handler = HotKeyBinding.GetInstance();
            playButtonId = handler.Register(IntPtr.Zero, System.Windows.Forms.Keys.MediaPlayPause, ModifierKeys.None, Player.PlayPauseCommand);
            nextButtonId = handler.Register(IntPtr.Zero, System.Windows.Forms.Keys.MediaNextTrack, ModifierKeys.None, Player.NextCommand);
            prevButtonId = handler.Register(IntPtr.Zero, System.Windows.Forms.Keys.MediaPreviousTrack, ModifierKeys.None, Player.PreviousCommand);
        } 
        /// <summary>
        /// Unbind media function buttons after end session of application.
        /// </summary>
        private static void UnregisterGlobalKeyBinding()
        {
            var handler = HotKeyBinding.GetInstance();
            handler.Unregister(IntPtr.Zero, playButtonId);
            handler.Unregister(IntPtr.Zero, nextButtonId);
            handler.Unregister(IntPtr.Zero, prevButtonId);
        }
        #endregion
        #region Application restart and end session controls
        /// <summary>
        /// Restart instance application.
        /// </summary>
        public static void Restart()
        {
            Instance.RequestRestart = true;
            Instance.Shutdown();
        }
        /// <summary>
        /// Reload Main-Window.
        /// </summary>
        public static void RestartWindow()
        {
            MainWindowClass.DonotShutdownApp(true);
            MainWindowClass.Close();
            PrepareAndShowWindow();
        }
        /// <summary>
        /// Application session end event implements.
        /// For unbind events, save settings or etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            UnregisterGlobalKeyBinding();
            HotKeyBinding.StopComponent();
            GlobalViewModel.GetInstance().RequestSaveSettings();
            BassEngine.RequestSaveSettings();
            Player.UnbindBassEvents(BassHost);
            OneInstanceMutexLock = null;
            if (RequestRestart)
            {
                using (var p = new Process { StartInfo = new ProcessStartInfo(AppPath) })
                {
                    p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                    p.Start();
                }
            }
        }
        #endregion
        #region PInvokes
        /// <summary>
        /// ! Function will work only in Windows OS !
        /// Create a console and attach it with application.
        /// </summary>
        [DllImport("Kernel32.dll")]
        private static extern void AllocConsole();
        /// <summary>
        /// ! Function will work only in Windows OS !
        /// Attach application to console. It works very ugly but IT JUST WORKS!
        /// </summary>
        [DllImport("Kernel32.dll")]
        private static extern bool AttachConsole(int processId);
        #endregion
    }
#pragma warning restore CA1060, CA1001
}
