# Commits and releases

Notable changes go in [CHANGELOG.md](CHANGELOG.md); use
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/) categories: Added,
Changed, Deprecated, Removed, Fixed and Security. Keep an Unreleased section,
omit empty categories, and write changes for users rather than copying git logs.
Preserve historical entries and dates.

## Commitizen

Use Python 3.10+ and [uv](https://docs.astral.sh/uv/getting-started/installation/),
or install the same pinned Commitizen with pip in a virtual environment.
Run these commands from the repository root:

```sh
uvx --from commitizen==4.19.2 cz commit
uvx --from commitizen==4.19.2 cz check --message "feat: describe the change"
uvx --from commitizen==4.19.2 cz version --project
```

Stage the intended files before `cz commit`. Regular git commits remain available.
Use `feat:` for compatible features, `fix:` for compatible fixes, and `!` or a
`BREAKING CHANGE:` footer for incompatible changes. Other maintenance types include
`docs:`, `chore:`, `test:`, `ci:`, `build:`, `refactor:` and `perf:`.
CI also verifies that pushed `v*` tags match the configured version and have
a dated release entry. CI checks PR titles as future squash commit subjects, including title edits;
it does not reject historical commits or routine automated direct pushes.
When merging, preserve the validated title as the squash commit subject.

## Semantic Versioning

The version source is [.cz.toml](.cz.toml), currently `1.2.6`, synchronized with
`src/AudioBand/Properties/AssemblyInfo.cs` (`AssemblyInformationalVersion`) and
`src/AudioBandInstaller/Product.wxs` (`ProductVersion`).
Only existing release tags and dated release notes establish published history.

For numbered releases, choose MAJOR for incompatible contract or profile schema changes,
MINOR for compatible features, and PATCH for compatible fixes. A release never
changes the meaning of an already published version.

## Prepare an approved release

Start from a clean branch with current tests passing and full release tags fetched
(`git fetch --tags`). Preview an explicit increment, then update version files:

```sh
uvx --from commitizen==4.19.2 cz bump --increment PATCH --dry-run --check-consistency
uvx --from commitizen==4.19.2 cz bump --increment PATCH --version-files-only --check-consistency
```

Use MINOR or MAJOR when appropriate. The `--version-files-only` command updates
`.cz.toml`, `src/AudioBand/Properties/AssemblyInfo.cs`, and `src/AudioBandInstaller/Product.wxs`
without creating a commit or tag. Review the diff.

Move only shipped Unreleased notes into `## [X.Y.Z] - YYYY-MM-DD` with the actual
release date, newest first; retain pending work in Unreleased. Add/update comparison
links between release tags and point Unreleased to `compare/vX.Y.Z...HEAD`.
Keep the notes curated: automatic changelog generation is disabled, so do not run
`cz changelog` or `cz bump --changelog` against this file.

Run the repository checks (`msbuild` in Release configuration and `vstest.console.exe`),
stage the changed version files and changelog explicitly, then create a
`chore(release): X.Y.Z` commit. Review/merge through the usual process.
Only after release approval, tag that reviewed release commit with `vX.Y.Z` and
push that specific tag.
