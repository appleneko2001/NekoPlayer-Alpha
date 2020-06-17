using Appleneko2001;
using DialogAdapters;
using ManagedBass;
using MaterialDesignThemes.Wpf;
using NekoPlayer.Core;
using NekoPlayer.Core.Engine;
using NekoPlayer.Core.Interfaces;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Globalization;
using NekoPlayer.Wpf.Dialogs;
using NekoPlayer.Wpf.ModelViews;
using PlayerNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NekoPlayer
{
    /// <summary>
    /// Part of control player host and context.
    /// </summary>
    public static class Player
    {
        #region Private variables
        private static TextFieldDialog textFieldDialog;
        private static GetTrackInfoDialog getTrackInfoDialog;
        private static IPlaylist selectedPlaylist;
        #endregion
        #region Other features implements
        public static int GetRandomizedSeed => (int)DateTime.UtcNow.Ticks;
        public static readonly RelayCommand GetRealTagCommand = new RelayCommand(OnGetRealTagCommand, ValidateSelectedItem);
        public static readonly RelayCommand ExplorerPathToCommand = new RelayCommand(OnExplorerPathToCommandExecuted, ValidateSelectedItem);
        public static readonly RelayCommand CreatePlaylistCommand = new RelayCommand(OnCreatePlaylistCommandExecuted);
        public static readonly RelayCommand UsePlaylistCommand = new RelayCommand(OnUsePlaylistCommandExecuted, ValidateUsePlaylistCommand);
        public static readonly RelayCommand EditNamePlaylistCommand = new RelayCommand(OnEditNamePlaylistCommandExecuted, ValidateEditNamePlaylistCommand);
        public static readonly RelayCommand DeletePlaylistCommand = new RelayCommand(OnDeletePlaylistCommandExecuted, ValidateDeletePlaylistCommand);
        public static readonly RelayCommand DeletePlayableOnPlaylistCommand = new RelayCommand(DeletePlayableOnPlaylistCommandExecuted, ValidateItemChangePlaylistAndSelectedItemCommand);
        private static async void OnGetRealTagCommand(object param)
        {
            var playable = param as IPlayable;
            if (getTrackInfoDialog is null)
                getTrackInfoDialog = new GetTrackInfoDialog(playable);
            var dialog = getTrackInfoDialog;
            await DialogHost.Show(dialog, null, null, (sender, args) =>
            {
                try
                {

                }
                catch (Exception e)
                {
                    ExceptMessage.PopupExcept(e);
                }
                finally
                {
                    getTrackInfoDialog.Dispose();
                    getTrackInfoDialog = null;
                }
            }).ConfigureAwait(false);

        }


        private static void OnExplorerPathToCommandExecuted(object param)
        {
            var playable = param as IPlayable;
            if (playable.IsLocalMedia)
            {
                System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(playable.GetMediaPath()));
            }
        }
        private static bool ValidateSelectedItem(object param)
        {
            return param is IPlayable;
        }

        private static async void OnCreatePlaylistCommandExecuted(object param)
        {
            if (textFieldDialog is null)
                textFieldDialog = new TextFieldDialog();
            var dialog = textFieldDialog;
            dialog.SetHeader(LanguageManager.RequestNode("playlist.create"));
            dialog.SetHint(LanguageManager.RequestNode("common.name"));
            await DialogHost.Show(dialog, null, null, (sender, args) =>
            {
                try
                {
                    if (args.Parameter != null)
                    {
                        if (bool.Parse((string)args.Parameter) == true)
                        {
                            var context = GlobalViewModel.GetInstance();
                            string name = textFieldDialog.Text;
                            context.CreatePlaylist(name);
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptMessage.PopupExcept(e);
                }
            }).ConfigureAwait(false);
        }
        private static bool ValidateItemChangePlaylistAndSelectedItemCommand(object param)
        {
            if (param is ListView)
            {
                ListView list = param as ListView;
                IPlaylist playlist = list.DataContext as IPlaylist;
                if (playlist != null)
                    return playlist.IsListItemChangeble && list.SelectedItem != null;
                else
                    return false;
            }
            else
                return false;
        }
        private static bool ValidateItemChangePlaylistCommand(object param)
        {
            if (param is ListView)
            {
                ListView list = param as ListView;
                IPlaylist playlist = list.DataContext as IPlaylist;
                if (playlist != null)
                    return playlist.IsListItemChangeble;
                else
                    return false;
            }
            else
                return false;
        }
        private static void DeletePlayableOnPlaylistCommandExecuted(object param)
        {
            if (param is ListView)
            {
                ListView list = param as ListView;
                IPlaylist playlist = list.DataContext as IPlaylist;
                if (!playlist.IsListItemChangeble)
                    throw new InvalidOperationException("This playlist is not changeble, we don't have any permission to change items in playlist.");
                List<IPlayable> collection = new List<IPlayable>();
                foreach (var item in list.SelectedItems)
                    collection.Add(item as IPlayable);
                if (collection.Count == 0)
                    return;
                string message = LanguageManager.RequestNode("playlist.deletedialog.message01").Replace("{trackname}",
                    collection.Count > 1 ? collection.Count + " " + LanguageManager.RequestNode("common.items") : collection[0].TrackInfo?.Title ?? collection[0].GetMediaPath(), StringComparison.InvariantCulture) + "\n";
                message += LanguageManager.RequestNode("playlist.deletedialog.message02");
                var result = MessageBox.Show(message, LanguageManager.RequestNode("playlist.deletedialog.header"), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Cancel)
                    return;
                foreach (var item in collection)
                {
                    playlist.DeletePlayable(item);
                    if (result == MessageBoxResult.Yes)
                        if (item.IsLocalMedia)
                            try
                            {
                                string path = item.GetMediaPath();
                                item.Close(true);
                                System.IO.File.Delete(path);
                            }
                            catch (Exception)
                            {
                            }
                }
                playlist.RequestSaveChanges();
                collection.Clear();
            }
        }
        private static bool ValidateEditNamePlaylistCommand(object param)
        {
            if (param is IPlaylist)
                return (param as IPlaylist).IsNameChangeble;
            return false;
        }
        private static async void OnEditNamePlaylistCommandExecuted(object param)
        {
            if (textFieldDialog is null)
                textFieldDialog = new TextFieldDialog();
            var dialog = textFieldDialog;
            dialog.SetHeader(LanguageManager.RequestNode("common.rename"));
            dialog.SetHint(LanguageManager.RequestNode("common.name"));
            var playlist = param as IPlaylist;
            dialog.Text = playlist?.Name ?? "";
            selectedPlaylist = param as IPlaylist;
            // Name length of the method is very looooonnnggggg....
            await DialogHost.Show(dialog, OnEditNamePlaylistCommandExecuted_DialogHostClosingEvent).ConfigureAwait(false);
        }
        private static void OnEditNamePlaylistCommandExecuted_DialogHostClosingEvent(object sender, DialogClosingEventArgs args)
        {
            try
            {
                if (args.Parameter != null)
                {
                    if (bool.Parse((string)args.Parameter) == true)
                    {
                        var context = GlobalViewModel.GetInstance();
                        string name = textFieldDialog.Text;
                        selectedPlaylist.RequestSetName(name);
                    }
                }
            }
            catch (Exception e)
            {
                ExceptMessage.PopupExcept(e);
            }
        }
        private static bool ValidateDeletePlaylistCommand(object param)
        {
            if (param is IPlaylist)
                return (param as IPlaylist).IsRemovable;
            return false;
        }
        private static void OnDeletePlaylistCommandExecuted(object param)
        {
            if (param is IPlaylist)
            {
                var playlist = param as IPlaylist;
                if (!playlist.IsRemovable)
                    return;
                var context = GlobalViewModel.GetInstance();
                context.DeletePlaylist(playlist.Identicator);
            }
        }
        #endregion
        #region Main commands and method implements
        public static readonly RelayCommand EjectCommand = new RelayCommand(OnEjectCommandExecuted);
        public static readonly RelayCommand MuteCommand = new RelayCommand(OnMuteCommandExecuted);
        public static readonly RelayCommand NextCommand = new RelayCommand((o) => OnNextCommandExecuted(true));
        public static readonly RelayCommand PreviousCommand = new RelayCommand(OnPreviousCommandExecuted);
        public static readonly RelayCommand PlayPauseCommand = new RelayCommand(OnPlayPauseCommandExecuted, ValidatePlayPauseCommand);

        public static void OnPlayPauseKeyboardCommandExecuted()
        {
            if (ValidatePlayPauseCommand(null))
            {
                OnPlayPauseCommandExecuted(null);
            }
        }

        public static readonly RelayCommand RepeatCommand = new RelayCommand(OnRepeatModeCommandExecuted);
        public static readonly RelayCommand ShuffleCommand = new RelayCommand(OnShuffleCommandExecuted);
        public static readonly RelayCommand VolumeChangeCommand = new RelayCommand(OnVolumeScrollCommandExecuted);

        #region Event binding, implements and unbinding
        public static void BindBassEvents(BassEngine engine)
        {
            if (engine == null)
                return;
            var context = GlobalViewModel.GetInstance();
            engine.OnUpdateTick += context.OnPlayStatementUpdate;
            engine.OnRequestNextTrack += Engine_OnRequestNextTrack;
            engine.OnLoadErrorTrack += Engine_OnLoadErrorTrack;
            engine.OnLoadedTrack += Engine_OnLoadedTrack;
            engine.OnReceiveCover += Engine_OnReceiveCover;
        }

        private static void Engine_OnReceiveCover(object sender, System.Windows.Media.Imaging.BitmapSource e)
        {
            var context = GlobalViewModel.GetInstance();
            context.SetCover(e);
        }

        private static void Engine_OnLoadedTrack(object sender, BassMediaInfo e)
        {
            var context = GlobalViewModel.GetInstance();
            context.SetNowPlaying(e.Playable);
        }

        private static void Engine_OnLoadErrorTrack(object sender, Tuple<IPlayable, BassException> args)
        {
            args.Item1.SetCorruptedState(true,args.Item2); 
        }

        private static void Engine_OnRequestNextTrack(object sender, EventArgs e)
        {
            OnNextCommandExecuted(false);
        }
        public static void UnbindBassEvents(BassEngine engine)
        {
            if (engine == null)
                return;
            var context = GlobalViewModel.GetInstance();
            engine.OnRequestNextTrack -= Engine_OnRequestNextTrack;
            engine.OnUpdateTick -= context.OnPlayStatementUpdate;
            engine.OnLoadErrorTrack -= Engine_OnLoadErrorTrack;
            engine.OnLoadedTrack -= Engine_OnLoadedTrack;
            engine.OnReceiveCover -= Engine_OnReceiveCover;
        }
        #endregion
        #region Methods for calls from other codes
        public static void PlayPlayableOnCurrentPlaylist(int index)
        {
            var context = GlobalViewModel.GetInstance();
            context.PlayableIndex = index;
            if (context.IsShuffleOn)
            {
                context.GetShuffledCurrentPlaylist(GetRandomizedSeed);
                context.PlayableIndex = 0;
            }
            try
            {
                var requestResult = context.GetCurrentTrack();
                if (requestResult.Item2)
                    PlayCommand(requestResult.Item1);
            }
            catch
            {
                OnNextCommandExecuted(true);
            }
        }
        #endregion
        #region Command method
        private static bool ValidateUsePlaylistCommand(object parameter)
        {
            return parameter is IPlaylist && (parameter as IPlaylist).Playables.Count > 0;
        }
        private static void OnUsePlaylistCommandExecuted(object parameter)
        {
            var context = GlobalViewModel.GetInstance();
            context.SetCurrentPlaylist(parameter as IPlaylist);
            context.PlayableIndex = -1;
            if (context.IsShuffleOn)
            {
                context.GetShuffledCurrentPlaylist(GetRandomizedSeed);
            }
            OnNextCommandExecuted(true);
        }

        private static void OnEjectCommandExecuted(object nothing)
        {
            using (OpenFileDialogAdapter dialog = new OpenFileDialogAdapter())
            {
                dialog.Filter = Utils.GetOpenDialogFilterPattern(LanguageManager.RequestNode("filetype.supported"));
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    App.BassHost.Open(dialog.FileName);
                }
            }
        }
        private static void OnMuteCommandExecuted(object nothing)
        {
            var context = GlobalViewModel.GetInstance();
            App.BassHost?.SetMute(!context.IsMuted);
        }
        private static void OnNextCommandExecuted(bool isUser)
        {
            var context = GlobalViewModel.GetInstance();
            var requestResult = context.GetNextTrack(isUser);
            try
            {
                if (requestResult.Item2)
                    PlayCommand(requestResult.Item1);
            }
            catch
            {
                OnNextCommandExecuted(true);
            }
        }
        private static bool ValidatePlayPauseCommand(object nothing)
        {
            return App.BassHost?.IsMediaLoaded ?? false;
        }
        private static void OnPlayPauseCommandExecuted(object nothing)
        {
            if (App.BassHost.IsPlaying)
            {
                App.BassHost.Pause();
            }
            else
            {
                App.BassHost.Resume();
            }
        }
        private static void OnPreviousCommandExecuted(object nothing)
        {
            var context = GlobalViewModel.GetInstance();
            var requestResult = context.GetPreviousTrack();
            try
            {
                if (requestResult.Item2)
                    PlayCommand(requestResult.Item1);
            }
            catch
            {
                OnPreviousCommandExecuted(null);
            }
        }
        private static void OnShuffleCommandExecuted(object nothing)
        {
            var context = GlobalViewModel.GetInstance();
            context.SetShuffleState(!context.IsShuffleOn);
            if (context.IsShuffleOn && context.CurrentPlaylist != null)
                context.GetShuffledCurrentPlaylist(GetRandomizedSeed);
        }
        private static void OnRepeatModeCommandExecuted(object nothing)
        {
            var context = GlobalViewModel.GetInstance();
            context.SetRepeatState(context.RepeatMode + 1);
        }
        private static void OnVolumeScrollCommandExecuted(object obj)
        {
            if (obj is string)
            {
                string s = (string)obj;
                int direction = s == "+" ? 1 : s == "-" ? -1 : 0;
                int multiply = 5;
                int mixed = direction * multiply;
                var context = GlobalViewModel.GetInstance();
                context.Volume += mixed;
            }
        }
        #endregion
        private static void PlayCommand(Core.IPlayable p)
        {
            App.BassHost.InitializeStream(p);
            App.BassHost.Play();
        }


        #endregion
    }
}
