using ManagedBass;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Wpf.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Core.Engine
{
    /// <summary>
    /// BASS Audio host controller.
    /// API Provider by ManagedBass. and it can used with Bass.Net too (Because I rewrote their code to compatible with ManagedBass by copyright reason)
    /// </summary>
    public sealed class BassEngine : IDisposable
    {
        public const int BASS_STREAM_NULL = 0;
        #region Dispose object structure
        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                TimerService.Stop();
                TimerService.Dispose();
                UnloadAllLoadedPlugins();
            }
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Initialize the bass audio engine.
        /// </summary>
        /// <param name="deviceId">Specify device, automatically choose default device if device object is wrong value or null</param>
        public BassEngine(IBassDevice dev = null)
        {
            IBassDevice device = dev;
            Errors? error = null;
            bool retried = false;
            retry0:
            if (device is null || device.DeviceId == null)
                device = BassDevice.GetDefaultDevice();
            if (device is null)
                throw new ArgumentNullException(null, "No output devices.");
            //device.Identicator
            if (!Bass.Init(device.Identicator, Flags: DeviceInitFlags.Default))
            {
                error = Bass.LastError;
                if (!retried)
                {
                    if (error == Errors.Device || error == Errors.Busy)
                    {
                        device = BassDevice.GetDefaultDevice();
                        retried = true;
                        goto retry0;
                    }
                    else
                    {
                        throw new ArgumentException(error.ToString());
                    }
                }
                else
                {
                    throw new ArgumentException(error.ToString());
                }
            }
            CurrentDevice = device;
            InitializeServices();
            if (error != null)
            {
                ExceptMessage.PopupExcept(error, additionalMessage: "We found a problem when initializing host, and we temporarily solved. You have to check the settings of output devices.");
            }
        }
        /// <summary>
        /// Initialize the bass audio engine.
        /// </summary>
        /// <param name="deviceId">Specify device id, automatically choose default device if device id is wrong value or null</param>
        public BassEngine(string deviceId = null)
        {
            bool useDefault = string.IsNullOrEmpty(deviceId);
            var device = BassDevice.GetBassDevice(deviceId);
            if (useDefault || device == null)
            {
                device = BassDevice.GetDefaultDevice();
            }
            Bass.Init(device.Identicator, Flags: DeviceInitFlags.Default);
            CurrentDevice = device;
            InitializeServices();
        }

        #endregion
        #region Private members
        private Dictionary<string, int> LoadedPlugins = new Dictionary<string, int>();
        private string[] pluginsName = new string[] { "bass_aac", "bass_alac", "bass_flac", "bass_wma" };
        private BassChannelStatus BassCurrentChannel = null;
        // Created for fixes bug "Sometimes you can't stop the previous track, and it keeps playing until it done."
        // Free it with delay
        private ObservableCollection<BassChannelStatus> BassChannels = new ObservableCollection<BassChannelStatus>();
        private IPlayable currentPlayable;
        private float m_Volume;
        private bool mediaisLoaded;
        private Timer TimerService; // Providing updates after event raises.
        private GlobalViewModel globalViewModel;

        private long mediaLength;
        #endregion
        #region Events implements
        public event EventHandler<double> OnUpdateTick;
        public event EventHandler OnStoppedPlay;
        public event EventHandler OnLoadTrack;
        public event EventHandler<BassMediaInfo> OnLoadedTrack;
        public event EventHandler<Tuple<IPlayable, BassException>> OnLoadErrorTrack;
        public event EventHandler OnRequestNextTrack;
        public event EventHandler<BitmapSource> OnReceiveCover;
        #endregion
        #region Public members (and with private setters)
        public IBassDevice CurrentDevice { get; private set; }
        public bool IsPlaying
        {
            get
            {
                if (BassCurrentChannel is null)
                    return false;
                return BassCurrentChannel.StreamId != BASS_STREAM_NULL ? (Bass.ChannelIsActive(BassCurrentChannel.StreamId) == PlaybackState.Playing) : false;
            }
        }
        public bool IsMediaLoaded
        {
            get
            {
                return mediaisLoaded ? (BassCurrentChannel != null) : false;
            }
        }
        #endregion
        private void InitializeServices()
        {
            LoadAllPlugins(System.IO.Path.Combine(Environment.CurrentDirectory, "BassModules"));
            globalViewModel = GlobalViewModel.GetInstance();
            globalViewModel.OnVolumeChanged += (s, e) => SetVolume(e);
            SetVolume(globalViewModel.Volume);
            TimerService = new Timer(100);
            TimerService.Elapsed += OnEventUpdate; // Bind method to timer for implement cycle updates
            BassChannels.CollectionChanged += BassChannels_CollectionChanged; // Prepare "delayed frees or stops track" to fix bug.
        }

        /// <summary>
        /// Delayed frees or stops the channel, take last one id for control.
        /// Do not call it yourself, it should be binded on BassChannels property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BassChannels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (BassChannels.Count == 0)
            {
                BassCurrentChannel = null;
            }
            else
            {
                BassCurrentChannel = BassChannels.Last();
            }

            if (e.OldItems != null)
                foreach (BassChannelStatus obj in e.OldItems)
                {
                    var id = obj.StreamId;
                    if (!Bass.StreamFree(id))
                    {
                        Bass.ChannelStop(id);
                        Bass.StreamFree(id);
                    }
                }
            if (BassChannels.Count > 1)
            {
                for (int i = 0; i < BassChannels.Count - 1; i++)
                {
                    var obj = BassChannels[i];
                    var id = obj.StreamId;
                    if (!Bass.StreamFree(id))
                    {
                        Bass.ChannelStop(id);
                        Bass.StreamFree(id);
                        BassChannels.Remove(obj);
                    }
                }
            }
        }
        private void OnEventUpdate(object sender, EventArgs e)
        {
            if (BassCurrentChannel is null)
                return;
            var BassStreamId = BassCurrentChannel.StreamId;
            var state = Bass.ChannelIsActive(BassStreamId);
            if (state == PlaybackState.Playing)
            {
                var sec = GetPosition();
                OnUpdateTick?.Invoke(this, sec);
            }
            else if (state == PlaybackState.Stopped)
            {
                Unload();
                OnStoppedPlay?.Invoke(this, null);
                OnRequestNextTrack?.Invoke(this, null);
            }
            if (currentPlayable.MediaFormat == Appleneko2001.AudioHeaderData.Unsupported)
            {
                Unload();
                OnStoppedPlay?.Invoke(this, null);
                OnRequestNextTrack?.Invoke(this, null);
            }
        }
        /// <summary>
        /// Save BASS Audio library settings to container.
        /// For now there can save selected output device only.
        /// </summary>
        public static void RequestSaveSettings()
        {
            SettingsManager.SetValue("SelectedOutputDevice", Bass.GetDeviceInfo(Bass.CurrentDevice).Driver);
        }
        public void Open(string path)
        {
            try
            {
                IPlayable media = new Playable(path);
                InitializeStream(media);
                Play();
            }
            catch (Exception e)
            {
                ExceptMessage.PopupExcept(e, false);
            }
        }
        public void InitializeStream(IPlayable file)
        {
            try
            {
                if (file != null || !file.Ready)
                    file.CheckStatus();
                if (file is null || !file.Ready || file.MediaFormat == Appleneko2001.AudioHeaderData.Unsupported || file.MediaFormat == Appleneko2001.AudioHeaderData.Error)
                    throw new ArgumentNullException(nameof(file));
                if (mediaisLoaded)
                {
                    Unload();
                }
                var cover = file.TrackInfo.AlbumImage;
                bool AllowFullBuffered = SettingsManager.GetValue<bool>("FullBufferedRead", false); 
                var channelId = Bass.CreateStream(AllowFullBuffered ? StreamSystem.Buffer : StreamSystem.NoBuffer, BassFlags.Default, file.GetBassStream());
                //(AllowFullBuffered ? BASSStreamSystem.STREAMFILE_BUFFER : BASSStreamSystem.STREAMFILE_NOBUFFER, BASSFlag.BASS_DEFAULT, file.GetBassStream(), IntPtr.Zero);
                if (channelId != BASS_STREAM_NULL)
                {
                    mediaisLoaded = true;
                    mediaLength = Bass.ChannelGetLength(channelId);
                    BassChannels.Add(new BassChannelStatus(channelId));
                    SetVolumeBass();
                    currentPlayable = file;
                    OnLoadedTrack?.Invoke(this, new BassMediaInfo(file, Bass.ChannelGetInfo(channelId)));
                    OnReceiveCover?.Invoke(this, cover);
                }
                else
                {
                    throw new BassException();
                }
            }
            catch (BassException e)
            {
                ExceptMessage.PrintConsole(e);
                OnLoadErrorTrack?.Invoke(this, new Tuple<IPlayable, BassException>(file, e));
            }
            catch (Exception e)
            {
                ExceptMessage.PopupExcept(e, false);
            }
        }

        #region Player controls
        public void Play()
        {
            if (!mediaisLoaded)
                return;
            Resume();
        }
        public void Resume()
        {
            if (BassCurrentChannel is null)
                return;
            var BassStreamId = BassCurrentChannel.StreamId;
            if (!IsPlaying)
            {
                Appleneko2001.Utils.AssertIfFail(Bass.ChannelPlay(BassStreamId, false), true);
                globalViewModel.SetPlayingState(true);
                TimerService.Start();
            }
        }
        public void Pause()
        {
            if (BassCurrentChannel is null)
                return;
            var BassStreamId = BassCurrentChannel.StreamId;
            if (IsPlaying)
            {
                Appleneko2001.Utils.AssertIfFail(Bass.ChannelPause(BassStreamId), true);
                globalViewModel.SetPlayingState(false);
                TimerService.Stop();
            }
        }
        public void Stop()
        {
            if (BassCurrentChannel is null)
                return;
            var BassStreamId = BassCurrentChannel.StreamId;
            Appleneko2001.Utils.AssertIfFail(Bass.ChannelStop(BassStreamId), true);
            globalViewModel.SetPlayingState(false);
            TimerService.Stop();
            globalViewModel.CurrentPosition = 0.0;
        }
        public void SetVolume(int value)
        {
            m_Volume = value / 100f;
            if (mediaisLoaded)
            {
                SetVolumeBass();
            }
        }
        public void SetMute(bool muted)
        {
            globalViewModel.SetMutedState(muted);
            SetVolumeBass();
        }
        private void SetVolumeBass()
        {
            if (BassCurrentChannel is null)
                return;
            var BassStreamId = BassCurrentChannel?.StreamId ?? BASS_STREAM_NULL;
            Appleneko2001.Utils.AssertIfFail(Bass.ChannelSetAttribute(BassStreamId, ChannelAttribute.Volume,
                (globalViewModel.IsMuted ? 0.0f : 1.0f) * m_Volume), true);
        }
        public double GetPosition()
        {
            if (BassCurrentChannel is null)
                return 0.0;
            var BassStreamId = BassCurrentChannel.StreamId;
            var position = Bass.ChannelGetPosition(BassStreamId, PositionFlags.Bytes);
            return Bass.ChannelBytes2Seconds(BassStreamId, position);
        }
        public void SetPosition(double newValue)
        {
            if (BassCurrentChannel is null)
                return;
            Object lockObj = new Object();
            using (TimedLock.Lock(lockObj, TimeSpan.FromSeconds(5)))
            {
                var BassStreamId = BassCurrentChannel.StreamId;
                var pos = Bass.ChannelSeconds2Bytes(BassStreamId, newValue);
                if (pos >= mediaLength) // Trying to prevent error position LMAO (Stupid way)
                    pos = mediaLength - 4;
                bool result = Bass.ChannelSetPosition(BassStreamId, pos);
                Appleneko2001.Utils.AssertIfFail(result, false, new Errors[1] { Errors.Position });
            }
        }
        public void Unload()
        {
            if (BassCurrentChannel is null)
                return;
            BassChannels.Remove(BassCurrentChannel);
            globalViewModel.SetPlayingState(false);
            currentPlayable.Close();
            TimerService.Stop();
            globalViewModel.CurrentPosition = 0.0;
        }
        #endregion
        public void SetDevice(IBassDevice device)
        {
            try
            {
                if (device is null)
                    return;
                double pos = 0;
                bool isPlayingState = IsPlaying, mediaisLoaded = false;
                if (BassCurrentChannel != null)
                {
                    pos = GetPosition();
                    mediaisLoaded = true;
                    Pause();
                    Unload();
                }
                Bass.Free();
                if (!Bass.Init(device.Identicator, Flags: DeviceInitFlags.Default))
                {
                    var errCode = Bass.LastError;
                    if (errCode == Errors.Init || errCode == Errors.Device || errCode == Errors.Busy)
                    {
                        ExceptMessage.PopupExcept(errCode, false);
                        device = BassDevice.GetDefaultDevice();
                        Appleneko2001.Utils.AssertIfFail(Bass.Init(device.Identicator, Flags: DeviceInitFlags.Default));
                        if (mediaisLoaded)
                        {
                            InitializeStream(currentPlayable);
                            globalViewModel.CurrentPosition = pos;
                            SetPosition(pos);
                            if (isPlayingState)
                                Play();
                        }
                        SettingsManager.SetValue("SelectedOutputDevice", device);
                    }
                    else if(errCode != Errors.OK)
                    {
                        ExceptMessage.PopupExcept(errCode, false);
                    }
                }
                else
                {
                    if (mediaisLoaded)
                    {
                        InitializeStream(currentPlayable);
                        globalViewModel.CurrentPosition = pos;
                        SetPosition(pos);
                        if (isPlayingState)
                            Play();
                    }
                }
            }
            catch (Exception e)
            {
                ExceptMessage.PrintConsole(e);
            }
        }

        /// <summary>
        /// Load all required BASS plugins.
        /// </summary>
        /// <param name="path">The BASS plugins directory</param>
        private void LoadAllPlugins(string path)
        {
            string message = "We can't load some required plugins:\n\n";
            bool error = false;
            foreach (var plugin in pluginsName)
            {
                try
                {
                    var id = Bass.PluginLoad(System.IO.Path.Combine(path, plugin + ".dll"));
                    var info = Bass.PluginGetInfo(id);
                    LoadedPlugins.Add(plugin, id);
                }
                catch (Exception e)
                {
                    message += e.Message + '\n';
                    error = true;
                }
            }
            if (error)
                ExceptMessage.PopupExcept(message);
        }
        /// <summary>
        /// Free all loaded BASS plugins.
        /// </summary>
        /// <returns>Return a list if we can't unload some plugins.</returns>
        private List<string> UnloadAllLoadedPlugins()
        {
            var Fails = new List<string>();
            var Removes = new List<string>();
            foreach (var plugin in LoadedPlugins)
            {
                if (!Bass.PluginFree(plugin.Value))
                {
                    Fails.Add(plugin.Key);
                }
                else
                    Removes.Add(plugin.Key);
            }
            foreach (var p in Removes)
                LoadedPlugins.Remove(p);
            return Fails;
        }
    }
}