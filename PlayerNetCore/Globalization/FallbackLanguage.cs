using NekoPlayer.Globalization.Interfaces;
using System.Collections.Generic;

namespace NekoPlayer.Globalization
{
    public class FallbackLanguage : ILanguage
    {
        private bool ready = false;
        public Dictionary<string, string> nodes;
        public FallbackLanguage()
        {
            Load();
        }
        public bool ContainNode(string nodeKey)
        {
            return nodes.ContainsKey(nodeKey?.ToLower(null) ?? "");
        }
        public string GetName() => "English (en-US, Fallback)";
        public string GetNode(string node)
        {
            if (!ready)
                return "Language pack are not ready.";
            return ContainNode(node) ? nodes[node] : node;
        }
        public bool IsReady() => ready;
        public void Load()
        {
            if (ready)
                return;
            nodes = new Dictionary<string, string>()
            {
                    { "error.working.header","An error occurred when working: " },
                    { "error.common.header","An error occurred when program running: " },
                    { "error.critical.header","An critical failure occurred when program running:" },
                    { "error.critical.suffix","Program will close. You can report critical error messages, procedure and logs to developer for improvement and crash fixes." },
                    { "error.bass.header","BASS audio playback engine reported error: " },

                    { "action.play","Play" },

                    { "common.selected", "Selected" },
                    { "common.search", "Search" },
                    { "common.searching", "Searching" },
                    { "common.result", "Result: " },
                    { "common.warning", "Warning" },
                    { "common.error", "Error" },
                    { "common.ok", "OK" },
                    { "common.cancel", "Cancel" },
                    { "common.duration", "Duration" },
                    { "common.close", "Close" },
                    { "common.name", "Name" },
                    { "common.yes", "Yes" },
                    { "common.no", "No" },
                    { "common.on", "On" },
                    { "common.off", "Off" },
                    { "common.items", "Items" },
                    { "common.delete", "Delete" },
                    { "common.rename", "Rename" },
                    { "common.tracks", "tracks" },
                    { "common.explorerto", "Explore to parent path" },

                    { "filetype.supported", "Supported audio file types" },
                    { "filetype.mp3", "MPEG-Layer 3 Audio" },
                    { "filetype.flac", "FLAC Lossless media" },
                    { "filetype.wma", "Windows media audio" },
                    { "filetype.m4a", "MPEG-Layer 4 Audio or Apple lossless" },
                    { "filetype.wav", "Wave audio" },

                    { "home.header", "Home" },

                    { "program.header", "NekoPlayer Alpha" },
                    { "program.version", "Version" },

                    { "playable.unknown","Unknown" },
                    { "playable.album","Album" },
                    { "playable.artist","Artist" },
                    { "playable.artistalbum","Artist Album" },
                    { "playable.title","Title" },

                    { "player.shuffle","Shuffle mode" },
                    { "player.repeat","Repeat mode" },
                    { "player.repeatone","Endless repeat one track :)" },
                    { "player.repeatplaylist","Repeat playlist" },
                    { "player.prevsong","Previous track" },
                    { "player.nextsong","Next track" },
                    { "player.volume","Volume" },
                    { "player.mute","Mute" },
                    { "player.loading","Loading..." },
                    { "player.seekbar","Seek bar" },
                    { "player.dropheretoplay","Drop media file here to play" },

                    { "playlist","Playlist" },
                    { "playlist.header","All playlists" },
                    { "playlist.nowplaying","Now playing list" },
                    { "playlist.recent","Recently played" },
                    { "playlist.create","Create a playlist" },
                    { "playlist.newplaylist.header","New playlist" },
                    { "playlist.deletedialog.header","Confirm action" },
                    { "playlist.deletedialog.message01","Are you sure to remove track {trackname}, and remove file?" },
                    { "playlist.deletedialog.message02","Yes - remove and delete file (cannot be undone). \nNo - just remove this from playlist. \nCancel - cancel action." },

                    { "settings.header", "Settings" },
                    { "settings.stopwhennowindow", "Close program when window closed" },
                    { "settings.language.header", "Language" },
                    { "settings.language.caption", "Use other language to changes texts." },
                    { "settings.language.changedmessage", "You have to restart the program to completely changes." },
                    { "settings.darktheme.header", "Use darken theme" },
                    { "settings.darktheme.caption", "Sounds like great! right?" },
                    { "settings.themes.header", "Themes" },
                    { "settings.globalization.header", "Globalization" },
                    { "settings.miscellaneous.header", "Miscellaneous" },
                    { "settings.caches.header", "Caches" },
                    { "settings.fullbufferedread.header", "Allow Full-Buffered reading track" },
                    { "settings.fullbufferedread.caption", "For less latency while playing, BUT HEAVY STORING TO RAM MEMORY may makes your computer slow down or some programs cannot be started.\r\nChanges will affected on next track or reopen track." },
                    { "settings.downsizewhengetcoveronline.header", "Downsize image cache" },
                    { "settings.downsizewhengetcoveronline.caption", "Resize and optimize cover image as PNG format for reduce RAM usage after get network tags.\r\nTechnology image processing by SixLabors.ImageSharp." },
                    { "settings.selectedoutputdevice.header", "Selected output device" },
                    { "settings.selectedoutputdevice.caption", "Choose other output devices to play music." },
                    { "settings.previewalbumimagesize.header", "Size of full show cover" },
                    { "settings.cachemanage.header", "Manage disk usages of caches" },
                    { "settings.cachemanage.caption",  "Show usages and clear on next startup." },
                    { "settings.linkcorrection.header", "Link correction" },
                    { "settings.linkcorrection.caption",  "Correct all hash linking (Player window will reload)" },
                    { "settings.downsizecachecovernow.header", "Downsize cached cover image" },
                    { "settings.downsizecachecovernow.caption", "Resize and optimize every cached cover image now (May takes more time for processing images, and encode it as PNG format to reduce RAM usage).\r\nTechnology image processing by SixLabors.ImageSharp." },


                    { "validaterule.ok", "That's ok!" },
                    { "validaterule.emptyfield", "Field is required and can't be empty." },
                    { "validaterule.matchedanother", "This text is used by other object." },

                    { "dialog.gettrackinfo.entry","Load tags from network" },
                    { "dialog.gettrackinfo.header","Load track tags from online database" },
                    { "dialog.gettrackinfo.database_source","Database source" },
                    { "dialog.gettrackinfo.track_name","Track name" },
                    { "dialog.gettrackinfo.track_artist","Track artist" },
                    { "dialog.gettrackinfo.track_album","Track album" },
                    { "dialog.gettrackinfo.write_to_file","Write tag to file" },
                    { "dialog.gettrackinfo.working","Downloading details of track and cover, please wait..." },
            };
            ready = true;
        }

        public void Unload()
        {
        }

        public string Geti18nCode() => "en-US";

        public string GetCreatorName() => "github@appleneko2001";
    }
}
