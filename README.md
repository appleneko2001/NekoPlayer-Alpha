# NekoPlayer Alpha
## ! Project still incomplete and unstable, could be use QT & C++ instead on future updates !

An WPF music player with Material design. 
UI are referenced from Google Play music.

## Features
* Play tracks from local storages
* Now playlist queues show (Incomplete)
* Load tags from network source (Only one API source: Netease Cloud Music)
* Create/Remove Playlist
* Shuffle mode and repeat
* Globalization support (create/modify a language pack to support more languages)
* Media key binding (Play/Pause, Previous and Next track button control support)
* Network tags, cover image caching and management (clear or show usage)
* Select output device to use other device instead
* Full-Buffering or Streaming data from local storage mode
* Dark theme
* Seek position
* Zoom up cover by stay cursor to cover image on dock-panel
* Auto-save changes of playlist after close window or add/delete items on playlist

## Compile
> For compile you need .Net Core 3.0, MaterialDesignInXAML (Modified) and Visual Studio 2019
- Download this repository and modded MaterialDesign toolkit and unpack it both
- Open solution file by Visual Studio 2019
- Remove old MaterialDesignInXAML project and re-add unpacked toolkit project
- Select Release solution configuration and Run (or Compile)

## How to use
- Create playlist by right-button on background main page
- Drag a music file to playlist, multi-drag are allowed
- Click playlist and press "Play" or double click item of list

## Known issues
- Wastes RAM while image processing (downsize)
- Sometimes could be buggy
- Smoothless by animation of transitioner (Recommend use Release build option instead could be much better)
- Seek will lose while choose device and Allow Full-Buffering are activated

## Used library on this project
- Bass, ManagedBass (Core of audio player, on older version of this project are used Bass.Net but for some reasons I used ManagedBass instead. I'm so sorry about betray Bass.Net library)
- SixLabors.ImageSharp (Cover image processing)
- Linq2DB (Cache Management)
- DialogAdapters (Dialogs, could deprecated on future updates)
- NeteaseCloudMusicApi (NCMApi Provider)
- taglib-sharp (Load track informations)
- ZeroFormatter (Serialize/Deserialize object, could deprecated on future updates)
- Newtonsoft.Json (Serialize/Deserialize JSON object)
- MaterialDesignInXAML (Wpf Styles)
- Hardcodet.Wpf.TaskbarNotification (System-tray on WPF)
