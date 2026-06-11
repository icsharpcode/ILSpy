# macOS release flow

CI (the `macOS` job in `build-ilspy.yml`) produces an **unsigned** zipped `ILSpy.app`
artifact. All signing happens offline on the release manager's machine; no signing
secrets exist in any CI.

Users who run the unsigned zip directly must clear the quarantine flag once:
`xattr -dr com.apple.quarantine ILSpy.app`. The dmg below is the signed distribution
channel that avoids this.

## Creating the release dmg

1. Download the `ILSpy macOS arm64 <version> (Release)` artifact from the build run on
   the release commit and unzip it.
2. Build, sign, and notarize the dmg (see `create-dmg.ps1` for parameter details):

   ```
   ./create-dmg.ps1 -AppPath ./ILSpy.app -Version <version> `
       -SigningIdentity 'Developer ID Application: <name> (<team>)' `
       -Notarize -NotaryProfile <notarytool profile>
   ```

   Omit `-SigningIdentity`/`-Notarize` to produce an unsigned dmg for local testing.
3. Attach `ILSpy-<version>.dmg` to the manually created GitHub release. The dmg file
   name must match the URL pattern in the Homebrew cask (`../homebrew/ilspy.rb`).
4. Update the Homebrew cask with the new version and the sha256 the script printed
   (see `../homebrew/README.md`).
