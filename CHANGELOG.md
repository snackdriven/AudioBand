# Changelog

All notable changes to this project will be documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and numbered releases follow [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Commitizen configuration (`.cz.toml`), release workflow (`RELEASE.md`), and CI validation (`.github/workflows/release-conventions.yml`) for Conventional Commits, SemVer consistency, and curated Keep a Changelog notes.
- Regression test coverage for `SemanticVersion` parsing (null/empty inputs, pre-release suffixes, release title prefixes) and `AppSettings` startup version persistence.

### Fixed

- Hardened `SemanticVersion` parsing against null/empty strings, release title prefixes (`AudioBand v1.2.6`), and SemVer pre-release/build metadata (`1.2.6-beta.1`) without throwing `ArgumentNullException` or `FormatException`.
- Guarded `GitHubHelper.IsOnLatestVersionAsync` and `GetLatestDownloadUrlAsync` when GitHub release queries return null or use tag names without spaces.
- Persisted `AudioBandSettings.LastKnownVersion` before initial profile selection in `AppSettings` so user customizations to `Default Profile` are not overwritten on subsequent startups.
- Prevented `AudioBandToolbarViewModel` from overwriting `LastNonIdleProfileName` with `Idle` during startup and guarded null `SelectedProfile` logging.
- Fixed `PersistentSettings` popup singleton access on corrupted `settings.json`, invalid profile file deletion path handling, and `_isBusy` reset in `WriteProfiles`.
- Clamped 15-second skip/rewind mouse bindings to valid `[0, SongLength]` bounds and avoided `FindWindow(null, null)` calls when launching an unstarted media player.
- Guarded `SpotifyAudioSource` volume updates when the API client is uninitialized or offline, and cleared `MusicBeeAudioSource` playback state when MusicBee exits.

## [1.2.6] - 2026-10-21

### Added

- Regression test suite (`BugFixRegressionTests`) covering audio source switching, profile naming collisions, profile deletion fallbacks, and automatic idle profile transitions.

### Changed

- Updated Spotify OAuth redirect URI to `http://127.0.0.1:<CallbackPort>/callback` (default port `8888`) to comply with Spotify's April/November 2025 redirect URI security requirements.
- Configured `SpotifyClient` with `AuthorizationCodeAuthenticator` so hourly OAuth access tokens refresh automatically without re-opening the browser.
- Prioritized `Spotify client ID` and `Spotify client secret` settings ahead of `Spotify RefreshToken` during startup settings hydration.
- Replaced hardcoded Visual Studio 2019 CI paths in `.github/workflows/cicd.yml` with `actions/checkout@v4`, `microsoft/setup-msbuild@v2`, `NuGet/setup-nuget@v2`, and `vstest.console.exe`.

### Fixed

- Recovered from revoked or expired Spotify refresh tokens (`invalid_grant` / `invalid_client`) by clearing the stale token and initiating a fresh OAuth flow, with a 30-second backoff when offline.
- Fixed Spotify podcast episode playback (`ItemType.Episode`) overwriting `_currentItemIsTrack` or calling `Library.CheckTracks` on episode IDs.
- Fixed `SpotifyControls` desktop window resolution for modern Chromium-based Spotify builds (`Chrome_WidgetWin_0` / `Chrome_WidgetWin_1`).
- Unsubscribed previous audio source event handlers in `AudioSession` when switching sources to prevent duplicate updates and memory leaks.
- Preserved `LastNonIdleProfileName` when automatic idle profile switching activates, and avoided resetting `IdleProfileName` when deleting unrelated profiles.
- Fixed `UserProfile.GetUniqueProfileName` infinite loop when a duplicate profile name already exists.
- Clamped volume adjustments to `[0, 100]` across `SpotifyAudioSource`, `Win10AudioSource`, and `iTunesAudioSource`.
- Corrected Win32 `GetWindowThreadProcessId` P/Invoke signature to use `uint` thread IDs.

## [1.2.5] - 2025-05-10

### Changed

- Updated Spotify local authentication address handling in preparation for `localhost` redirect deprecation.

[Unreleased]: https://github.com/snackdriven/AudioBand/compare/8047220...HEAD
[1.2.6]: https://github.com/snackdriven/AudioBand/commits/8047220
[1.2.5]: https://github.com/AudioBand/AudioBand/releases/tag/v1.2.5
