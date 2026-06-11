# Template for the ILSpy Homebrew cask. The live copy belongs in the tap repository
# (icsharpcode/homebrew-ilspy) at Casks/ilspy.rb; see README.md next to this file.
cask "ilspy" do
  version "REPLACE_ME" # e.g. 11.0.0.8948
  sha256 "REPLACE_ME"  # printed by create-dmg.ps1, or: shasum -a 256 ILSpy-<version>.dmg

  url "https://github.com/icsharpcode/ILSpy/releases/download/v#{version}/ILSpy-#{version}.dmg"
  name "ILSpy"
  desc ".NET assembly browser and decompiler"
  homepage "https://github.com/icsharpcode/ILSpy"

  depends_on macos: ">= :big_sur"
  depends_on arch: :arm64

  app "ILSpy.app"
end
