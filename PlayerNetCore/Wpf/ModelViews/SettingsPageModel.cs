using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using NekoPlayer.Globalization;
using NekoPlayer.Wpf.ItemsControlViews;
using NekoPlayer.Core.Engine;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Containers;
using NekoPlayer.Core;
using NekoPlayer.Wpf.Dialogs;
using MaterialDesignThemes.Wpf;

namespace NekoPlayer.Wpf.ModelViews
{
    /// <summary>
    /// Use this attribute with property to make visible when running GetProperties() with IsDefined()
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingObjectAttribute : Attribute
    {
        /// This attribute is created for add a flag to make finding properties easily.
    }
    public class SettingsPageModel : ViewModelBase
    {
        public CompositeCollection Menus { get; private set; }


        [SettingObject]
        public GroupControl Theming => new GroupControl(LanguageManager.RequestNode("settings.themes.header"), new object[] {
           new BooleanControl("UseDarkTheme", LanguageManager.RequestNode("settings.darktheme.header"),
           LanguageManager.RequestNode("settings.darktheme.caption"), false, onValueChanged: (value) => PlayerNetCore.App.ApplyDarkTheme(value))
        } );
        [SettingObject]
        public GroupControl Globalization => new GroupControl(LanguageManager.RequestNode("settings.globalization.header"), new object[] {
            new ComboBoxListControl("Language", LanguageManager.RequestNode("settings.language.header"),
            ComboBoxListControl.GenerateListFromGetLanguagePacks(LanguageManager.GetLanguagesPackList()),
            defValue: LanguageManager.FallbackDefaultLanguage,
            onDialogClosedEvent: (obj, e) => ApplyLanguage(obj))});
        [SettingObject]
        public GroupControl Miscellaneous => new GroupControl(LanguageManager.RequestNode("settings.miscellaneous.header"), new object[] {
        new BooleanControl("StopWhenNoWindow", LanguageManager.RequestNode("settings.stopwhennowindow"), null,false),
        new BooleanControl("FullBufferedRead", LanguageManager.RequestNode("settings.fullbufferedread.header"), LanguageManager.RequestNode("settings.fullbufferedread.caption"),false),
            SelectedOutputDevice, PreviewAlbumImageSize,
        new BooleanControl("DownsizeWhenGetCoverOnline",LanguageManager.RequestNode("settings.downsizewhengetcoveronline.header"), LanguageManager.RequestNode("settings.downsizewhengetcoveronline.caption"))});


        private ComboBoxListControl SelectedOutputDevice => new ComboBoxListControl(nameof(SelectedOutputDevice), LanguageManager.RequestNode("settings.selectedoutputdevice.header"),
            () => ComboBoxListControl.GenerateListFromGetBassDevices(BassDevice.GetPlaybackDevices()), LanguageManager.RequestNode("settings.selectedoutputdevice.caption"), null, MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh, null, 
            (obj, args)=> {
                PlayerNetCore.App.BassHost.SetDevice(BassDevice.GetBassDevice((string)SelectedOutputDevice.Value));
            });
        private ComboBoxListControl PreviewAlbumImageSize => new ComboBoxListControl(nameof(PreviewAlbumImageSize), LanguageManager.RequestNode("settings.previewalbumimagesize.header"), GetListSizeofPreviewIllust(), null, "240");

        [SettingObject]
        public GroupControl Caches => new GroupControl(LanguageManager.RequestNode("settings.caches.header"), 
            new object[] 
            { 
                new ButtonControl(LanguageManager.RequestNode("settings.cachemanage.header"), LanguageManager.RequestNode("settings.cachemanage.caption"), null, async () => {
                    CacheManagerDialog dialog = new CacheManagerDialog();
                    await DialogHost.Show(dialog,null, (sender, args) => {
                        dialog.OnDialogClosed();
                        dialog = null;
                    } ).ConfigureAwait(false);
                }) ,
                new ButtonControl(LanguageManager.RequestNode("settings.linkcorrection.header"), LanguageManager.RequestNode("settings.linkcorrection.caption"), null, ()=> {
                    try
                    {
                        CacheManager.HashLinkCorrection();
                        var global = GlobalViewModel.GetInstance();
                        foreach(var list in global.Playlists)
                        {
                            foreach(var item in list.Playables)
                            {
                                if(global.NowPlayingItem != item)
                                    if(item is Playable)
                                        (item as Playable).Load();
                            }
                        }
                        PlayerNetCore.App.RestartWindow();
                    }
                    catch(Exception e)
                    {
                        ExceptMessage.PopupExcept(e);
                    }
                }),
                new ButtonControl(LanguageManager.RequestNode("settings.downsizecachecovernow.header"), LanguageManager.RequestNode("settings.downsizecachecovernow.caption"),
                    PackIconKind.ImageSizeSelectLarge, () => CacheManager.DownsizeCover())
            });

       /* [SettingObject]
        public GroupControl Lyrics => new GroupControl(nameof(Lyrics), 
            new object[] 
            { 
                new BooleanControl("EnableLyric", "Enable lyric feature (Experimental)",
                    "Still a incompleted feature, can be more buggy... (Required reload window)", 
                    iconKind: MaterialDesignThemes.Wpf.PackIconKind.Build)
            });*/

        private static List<Dialogs.ComboBoxListItem> GetListSizeofPreviewIllust()
        {
            var list = new List<Dialogs.ComboBoxListItem>();
            var sizes = new int[] { 240, 360, 500, 600, 800 };
            foreach (var item in sizes)
            {
                list.Add(new Dialogs.ComboBoxListItem(item, $"{item}x{item}"));
            }
            return list;
        }
        private static void ApplyLanguage(object obj)
        {
            if(LanguageManager.SelectLanguage((obj as ComboBoxListControl).Value as string))
            {
                PlayerNetCore.App.RestartWindow();
            }
        }
        
        public SettingsPageModel()
        {
            // Get all properties with SettingObject attribute.
            var fields = GetType().GetProperties().Where((info) => info.IsDefined(typeof(SettingObjectAttribute),false));
            Menus = new CompositeCollection();
            foreach(var field in fields)
            {
                var obj = field.GetValue(this);
                Menus.Add(obj);
            }
            // Automatically add those buttons easily, right? 
            // just make all procedure works not very traditionally.
        }
    }
}
