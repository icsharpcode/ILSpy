# Homebrew cask distribution

ILSpy is distributed for macOS via a Homebrew *tap*: a separate repository named
`icsharpcode/homebrew-ilspy` containing nothing but the cask file at `Casks/ilspy.rb`.
Users then install with:

```
brew install --cask icsharpcode/ilspy/ilspy
```

The tap holds no secrets and runs no CI. Signing and notarization happen offline
(see `../macos/README.md`); the cask merely points at the signed dmg attached to the
GitHub release.

## Per-release update (manual)

1. Create the dmg and attach it to the GitHub release (`../macos/README.md`).
2. In the tap repo, edit `Casks/ilspy.rb`:
   - `version` — the release version (must match the dmg file name and release tag,
     see the `url` line).
   - `sha256` — printed by `create-dmg.ps1`, or `shasum -a 256 ILSpy-<version>.dmg`.
3. Commit and push. `brew upgrade --cask ilspy` picks it up immediately; there is no
   review process for casks in your own tap.

`ilspy.rb` in this directory is the template for the tap's cask file. If the release
tag or dmg naming scheme ever changes, adjust the `url` line accordingly.
