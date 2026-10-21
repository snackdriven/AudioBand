# AGENTS.md

## Project Shape

`AudioBand` is a Windows taskbar DeskBand (`IDeskBand` via `CSDeskBand` / WPF on `.NET Framework 4.8`) that displays song metadata and playback controls from pluggable audio sources (`SpotifyAudioSource`, `Win10AudioSource`, `iTunesAudioSource`, `MusicBeeAudioSource`).

Key entry points:

- `src/AudioBand/` - Core DeskBand, WPF UI (`UI/`), settings persistence (`Settings/`), and plugin loader (`AudioSource/`).
- `src/SpotifyAudioSource/` - Spotify Web API + `EmbedIOAuthServer` (`http://127.0.0.1:8888/callback`) and desktop `WM_APPCOMMAND` fallback.
- `src/Win10AudioSource/` - Windows Global System Media Transport Controls (`GSMTC`) audio source.
- `src/iTunesAudioSource/` - iTunes COM audio source.
- `src/MusicBeeAudioSource/` - MusicBee IPC audio source.
- `src/AudioBand.Test/` - xUnit + Moq unit and regression test suite.
- `src/AudioBandInstaller/` - WiX (`Product.wxs`) MSI installer definition.
- `CHANGELOG.md` and `RELEASE.md` - Keep a Changelog release history, SemVer policy, and Commitizen workflow.

## Safety Rules

- **Never build `AudioBand.csproj` in `Debug` mode unless you explicitly intend to restart `explorer.exe`.** `AudioBand.csproj` defines a `PreBuildEvent` that runs `taskkill /f /im explorer.exe` when `$(Configuration) == 'Debug'` and a `PostBuildEvent` that restarts Explorer. Always pass `/p:Configuration=Release` for normal builds and test runs.
- Keep `.cz.toml`, `src/AudioBand/Properties/AssemblyInfo.cs` (`AssemblyInformationalVersion`), and `src/AudioBandInstaller/Product.wxs` (`ProductVersion`) synchronized via Commitizen (`RELEASE.md`). Do not change `AssemblyVersion("1.0.0.0")` casually, as COM registration and test assembly bindings depend on it.

## Commands

Build and test (PowerShell / Windows with Visual Studio 2022 Build Tools):

```powershell
msbuild src/AudioBand/AudioBand.csproj /t:Restore /p:Configuration=Release
msbuild src/AudioBand/AudioBand.csproj /t:Build /p:Configuration=Release
msbuild src/SpotifyAudioSource/SpotifyAudioSource.csproj /t:Restore,Build /p:Configuration=Release
msbuild src/AudioBand.Test/AudioBand.Test.csproj /t:Restore,Build /p:Configuration=Release
vstest.console.exe src/AudioBand.Test/bin/Release/AudioBand.Test.dll
```

Release conventions check:

```sh
node --test .github/tests/jules-pr-titles.check.cjs
uvx --from commitizen==4.19.2 cz check --message "fix: describe the change"
```

## Jules PR titles

Before opening or updating a PR, set its title to `type: description` or
`type(scope): description`. Start with a lowercase Conventional Commit type,
not a leading emoji or bracketed task category. Use a lowercase imperative
description, such as `perf: optimize search`, `test: cover invalid inputs`,
`refactor: extract helpers`, or `fix: handle failed requests`.

Release conventions repairs recognized, same-repository Jules titles before
the existing title validator runs. This is a fallback; create a compliant
title directly. Existing valid titles and other contributors are unchanged.
Check the title-policy fixtures with
`node --test .github/tests/jules-pr-titles.check.cjs`.
