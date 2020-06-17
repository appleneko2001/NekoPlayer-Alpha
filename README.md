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
> For compile and run this project you need [.Net Core 3.0 SDK and Desktop Runtime](https://dotnet.microsoft.com/download/dotnet-core/3.0), [MaterialDesignInXAML (Modified)](https://github.com/appleneko2001/MaterialDesignInXaml-Mod) and [Visual Studio 2019](https://visualstudio.microsoft.com)
- Install all requirements if you haven't them 
- Download this repository and modded MaterialDesign toolkit and unpack it both
- Open solution file by Visual Studio 2019
- Remove old MaterialDesignInXAML project reference and add it again to correct path of toolkit
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
- Key binding are unavailable while iTunes are running

## Used libraries on this project
- [Bass](http://www.un4seen.com/bass.html), [ManagedBass](https://github.com/ManagedBass/ManagedBass) (Core of audio player, on older version of this project are used Bass.Net but for some reasons I used ManagedBass instead. I'm so sorry about betray Bass.Net library)
- [SixLabors.ImageSharp](https://sixlabors.com/products/imagesharp/) (Cover image processing)
- [Linq2DB](https://linq2db.github.io/) (Cache Management)
- [DialogAdapters](https://www.nuget.org/packages/DialogAdapters/) (Dialogs, could deprecated on future updates)
- [NeteaseCloudMusicApi](https://github.com/wwh1004/NeteaseCloudMusicApi) (NCMApi Provider)
- [taglib-sharp](https://github.com/mono/taglib-sharp) (Load track informations)
- [ZeroFormatter](https://github.com/neuecc/ZeroFormatter) (Serialize/Deserialize object, could deprecated on future updates)
- [Newtonsoft.Json](https://www.newtonsoft.com/json) (Serialize/Deserialize JSON object)
- [MaterialDesignInXAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) (Wpf Styles)
- [Hardcodet.Wpf.TaskbarNotification](http://www.hardcodet.net/wpf-notifyicon) (System-tray on WPF)
