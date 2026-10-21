<div align="center"><img src="logo/logotype_cover.svg"/></div><br/>

# Audio Band

Windows taskbar music toolbar (`IDeskBand`) for Spotify, Windows 10/11 Media Controls (GSMTC), iTunes, and MusicBee.

<img align="right" src="screenshots/demo.gif"/>

## What's in This Fork

- **Spotify OAuth & Token Refresh:** Updated for Spotify's loopback redirect requirement (`http://127.0.0.1:8888/callback`), automatic hourly token refresh, `invalid_grant` recovery, and podcast episode support.
- **Windows 11 Support:** Works on Windows 11 with [StartAllBack](https://www.startallback.com/) or [ExplorerPatcher](https://github.com/valinet/ExplorerPatcher) (`Right-click Taskbar -> Toolbars -> AudioBand`).
- **Stability & Bug Fixes:** Fixed audio source switching leaks, idle profile preservation, profile deletion/import edge cases, volume/seek clamping, and in-app update checks.

## More Info

- **[Releases & MSI Installer](https://github.com/snackdriven/AudioBand/releases)** — Download the latest `AudioBand.msi` (`v1.2.6`).
- **[CHANGELOG.md](CHANGELOG.md)** — Full list of fixes and release history.
- **[RELEASE.md](RELEASE.md) & [AGENTS.md](AGENTS.md)** — Build, test, SemVer, and Commitizen conventions.
- **[Original Documentation](https://audioband.github.io/AudioBand/audioband/index.html)** — Upstream user guide and profile customization docs.
- **[LICENSE](LICENSE)** & **[THIRD PARTY](LICENSE-3RD-PARTY)**
