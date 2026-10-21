<div align="center"><img src="logo/logotype_cover.svg"/></div><br/><br/>

# Audio Band

Audio Band displays song information and playback controls from various audio sources directly in the Windows Taskbar.

This fork updates [AudioBand/AudioBand](https://github.com/AudioBand/AudioBand) for modern **Spotify OAuth loopback requirements** (`http://127.0.0.1:8888/callback`), **Windows 11** (via [StartAllBack](https://www.startallback.com/) or [ExplorerPatcher](https://github.com/valinet/ExplorerPatcher)), and fixes core session, profile, and token-refresh bugs. See [CHANGELOG.md](CHANGELOG.md) for release notes and [Releases](https://github.com/snackdriven/AudioBand/releases) for the latest `.msi` installer.

<img align="right" src="screenshots/demo.gif"/>

## Features
- Displays currently playing song (album art, artist, title, progress)
- Control your music from the Taskbar (play/pause, previous/next, shuffle, repeat, volume, like/heart)
- Add support for your music player of choice through plugins
- Customize the look and layout entirely with profiles

## Supported Audio Sources
- **Spotify** *(requires Spotify Premium for API playback controls; desktop fallback supported for play/pause/next/prev)*
- **Windows 10 / 11 Media Controls (GSMTC)** ([List of Compatible Apps](https://github.com/ModernFlyouts-Community/ModernFlyouts/blob/main/docs/GSMTC-Support-And-Popular-Apps.md))
- **iTunes**
- **MusicBee**

## Quick Setup

### Windows 11 Taskbar
Windows 11 removed native `IDeskBand` support from the stock shell. Use a taskbar utility that restores classic toolbars, then right-click the Taskbar -> **Toolbars** -> **AudioBand**:
- **[StartAllBack](https://www.startallback.com/)** *(Enable enhanced classic taskbar)*
- **[ExplorerPatcher](https://github.com/valinet/ExplorerPatcher)** *(Taskbar style: Windows 10)*

### Spotify (`http://127.0.0.1:8888/callback`)
1. Open the [Spotify Developer Dashboard](https://developer.spotify.com/dashboard), create or select your app, and open **Settings**.
2. Add `http://127.0.0.1:8888/callback` under **Redirect URIs** and click **Save**.
3. Enter your **Client ID** and **Client secret** in **AudioBand Settings -> Spotify** (with **Callback Port** set to `8888`), click **Apply**, and complete the browser prompt.

## Building & Testing

> [!CAUTION]
> Building `AudioBand.csproj` in `Debug` mode runs a `PreBuildEvent` that kills `explorer.exe`. Always build with `/p:Configuration=Release` unless you intend to restart Explorer.

```powershell
msbuild src/AudioBand.sln /t:Restore,Build /p:Configuration=Release
vstest.console.exe src/AudioBand.Test/bin/Release/AudioBand.Test.dll
```

## Screenshots
![Screenshot 1](screenshots/screenshot1.png)

![Screenshot 2](./screenshots/screenshot.png)

![Screenshot 3](screenshots/custom-1.png)

## Links & License
- [CHANGELOG.md](CHANGELOG.md) & [RELEASE.md](RELEASE.md)
- [Original AudioBand Documentation](https://audioband.github.io/AudioBand/audioband/index.html)
- [LICENSE](LICENSE) & [THIRD PARTY](LICENSE-3RD-PARTY)

Logo originally by @mirzazulfan (edited by @SnakeStorm87)
