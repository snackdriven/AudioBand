<div align="center"><img src="logo/logotype_cover.svg"/></div><br/><br/>

# Audio Band (`snackdriven/AudioBand` fork)

Audio Band displays song information and playback controls from various audio sources directly in the Windows Taskbar.

This fork updates [AudioBand/AudioBand](https://github.com/AudioBand/AudioBand) (`v1.2.6`) for modern **Spotify OAuth loopback requirements** (`http://127.0.0.1:8888/callback`), **Windows 11** (via [StartAllBack](https://www.startallback.com/) or [ExplorerPatcher](https://github.com/valinet/ExplorerPatcher)), and fixes multiple core session, profile, and token-refresh bugs.

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

---

## What's Fixed in This Fork (`v1.2.6`)

### 1. Spotify OAuth (`127.0.0.1:8888`) & Token Refresh Fixes
Spotify deprecated `http://localhost` redirect URIs in 2025 and requires an explicit IPv4 loopback address (`127.0.0.1`) with an explicit port in the Spotify Developer Dashboard:
- **Loopback Redirect URI (`http://127.0.0.1:8888/callback`):** Updated `SpotifyAudioSource` to bind `EmbedIOAuthServer` to `http://127.0.0.1:<Callback Port>/callback` with a default `Callback Port` of **`8888`** (Spotify's dashboard rejects portless `http://127.0.0.1/callback` as insecure).
- **Automatic Hourly Token Refresh:** Restoring from a saved `RefreshToken` now configures `SpotifyClientConfig` with `AuthorizationCodeAuthenticator` (and proxy settings if enabled) instead of a static bearer token, preventing playback polling from dying after 1 hour.
- **`invalid_grant` Recovery & Offline Backoff:** When a saved `RefreshToken` is revoked or invalidated (`invalid_grant` / `invalid_client`), `SpotifyAudioSource` now automatically clears the stale token and launches a fresh OAuth flow instead of spamming `%TEMP%\AudioBand.log` every 1 second (~20 MB/day). Transient offline errors back off to a 30-second retry interval.
- **Startup Setting Priority & Auth Server Lifecycle:** Assigned `Priority = 10` to `Spotify Client ID` and `Spotify Client secret` so they load before `RefreshToken` (`Priority = 9`), preventing premature `Authorize()` errors on startup. Properly disposes previous `EmbedIOAuthServer` instances before re-binding.
- **Podcast Episodes & Local Tracks:** Added duplicate-item caching and null-safe `Show` metadata checks in `NotifyEpisodeUpdate` (fixing a bug where podcast artwork was re-downloaded over HTTP every second), and guarded `Library.CheckTracks` / `SaveTracks` / `RemoveTracks` so non-track items (podcasts and local files) don't trigger invalid Spotify API calls.
- **Desktop Media Key Fallback:** Fixed `SpotifyControls` (`TryPlay`, `TryPause`, `TryPrevious`, `TryNext`) so the Spotify window handle (`_spotifyHwnd`) is resolved on demand when falling back to `WM_APPCOMMAND`.

### 2. Core `AudioBand`, `Win10AudioSource`, and `iTunesAudioSource` Fixes
- **AudioSource Switch Event Leak (`AudioSession`):** Fixed `CurrentAudioSource` setter so switching audio sources unsubscribes event handlers from the *old* audio source rather than the new one.
- **Automatic Idle Profile Preservation (`AppSettings` & `GlobalSettingsViewModel`):** Fixed `AppSettings.SelectProfile` and `GlobalSettingsViewModel.OnEndEdit` overwriting `LastNonIdleProfileName` with `"Idle"` or stale values.
- **Profile Deletion & Import Fixes (`AppSettings` & `UserProfile`):** Fixed `AppSettings.DeleteProfile` reassigning `IdleProfileName` / `LastNonIdleProfileName` to the deleted profile when deleting the first profile in the dictionary, and fixed an infinite loop in `UserProfile.GetUniqueProfileName` when importing a duplicate profile name.
- **In-App Updater & Community Profile Repo (`GlobalSettingsViewModel`, `ProfileRepoViewModel`, `GitHubHelper`):** Fixed `WebClient` premature disposal during async downloads, removed hardcoded `C:\temp\` working directory in the MSI updater, fixed Windows backslash URL corruption when downloading nested community profile assets, and fixed `IsDownloading` getting stuck when a profile is already installed.
- **Popup Service, Volume Clamping & Win32 P/Invoke:** Added missing `return;` in `PopupService.ShowPopup` on missing resource keys, clamped mouse-wheel and mouse-binding volume adjustments to `[0, 100]`, and fixed `uint dwThreadId` signatures in `NativeMethods`.
- **Windows 10/11 GSMTC & iTunes Fixes:** Fixed `Win10AudioSource` startup timer race condition, per-second session event churn, and null `TryGetMediaPropertiesAsync()` dereference; wired up `NotifyRepeatMode()` in `iTunesAudioSource`.

---

## Setup Guide

### Windows 11 Taskbar Prerequisite
Windows 11 removed native support for classic `IDeskBand` taskbar toolbars in the stock shell. To use AudioBand on Windows 11, you need a taskbar utility that restores classic DeskBand/toolbar support, such as:
- **[StartAllBack](https://www.startallback.com/)** *(Enable enhanced classic taskbar -> right-click Taskbar -> Toolbars -> AudioBand)*
- **[ExplorerPatcher](https://github.com/valinet/ExplorerPatcher)** *(Taskbar style: Windows 10 -> right-click Taskbar -> Toolbars -> AudioBand)*

### Spotify Setup (`http://127.0.0.1:8888/callback`)
1. Go to the [Spotify Developer Dashboard](https://developer.spotify.com/dashboard) and log in.
2. Create an app (or open your existing AudioBand app) and go to **Settings**.
3. Under **Redirect URIs**, add:
   ```text
   http://127.0.0.1:8888/callback
   ```
   *(Remove any old `http://localhost/callback` entries, click **Add**, and click **Save** at the bottom).*
4. Copy your **Client ID** and **Client secret** into **AudioBand Settings -> Spotify** (make sure **Callback Port** is set to `8888`), and click **Apply**.
5. Complete the Spotify authorization prompt in your browser.

---

## Building & Running Tests Locally

This repository includes `src/Directory.Build.props` with `Microsoft.NETFramework.ReferenceAssemblies`, so it can be built with Visual Studio 2022 Build Tools (`MSBuild.exe`) even without the standalone `.NET Framework 4.8 Developer Pack` installed.

> [!CAUTION]
> Building `AudioBand.csproj` in `Debug` configuration executes a `PreBuildEvent` that kills `explorer.exe` (`taskkill /f /im explorer.exe`) to unlock `AudioBand.dll`. Always build with `/p:Configuration=Release` unless you intend to restart Explorer.

```powershell
# Restore NuGet packages and build in Release mode
msbuild src/AudioBand/AudioBand.csproj /t:Restore /p:Configuration=Release
msbuild src/AudioBand/AudioBand.csproj /t:Build /p:Configuration=Release
msbuild src/SpotifyAudioSource/SpotifyAudioSource.csproj /t:Restore,Build /p:Configuration=Release

# Build and run unit tests
msbuild src/AudioBand.Test/AudioBand.Test.csproj /t:Restore,Build /p:Configuration=Release
vstest.console.exe src/AudioBand.Test/bin/Release/AudioBand.Test.dll
```

## Screenshots
![Screenshot 1](screenshots/screenshot1.png)

![Screenshot 2](./screenshots/screenshot.png)

![Screenshot 3](screenshots/custom-1.png)

## Original Documentation & License
- [Original AudioBand Documentation](https://audioband.github.io/AudioBand/audioband/index.html)
- [LICENSE](LICENSE)
- [THIRD PARTY](LICENSE-3RD-PARTY)

Logo originally by @mirzazulfan (edited by @SnakeStorm87)
