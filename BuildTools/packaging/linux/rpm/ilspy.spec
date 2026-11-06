# Built by BuildTools/package-linux.ps1, which stages the complete payload tree
# (/opt/ilspy + launcher symlink + desktop integration) and passes it in via
#   rpmbuild -bb ilspy.spec --target x86_64
#     --define "_topdir <tmp>" --define "_version <version>" --define "_payload_dir <staged tree>"

# The payload is a prebuilt self-contained .NET publish; skip post-install processing
# (stripping the bundled native libraries breaks them) and debuginfo extraction.
%global __os_install_post %{nil}
%global debug_package %{nil}
%global _build_id_links none

Name: ilspy
Version: %{_version}
Release: 1
Summary: .NET assembly browser and decompiler
License: MIT
URL: https://github.com/icsharpcode/ILSpy
# The self-contained payload bundles its own .NET runtime; automatic dependency scanning
# would generate requires for every bundled shared object, so dependencies are listed
# manually instead.
AutoReqProv: no
# Native libraries are required by soname, not package name: the names differ per distro
# (openSUSE has libICE6 and libX11-6, Fedora has libICE and libX11), and the bundled .NET
# runtime only loads libssl 3, which Fedora 45+ (OpenSSL 4) ships as a compat package.
Requires: libX11.so.6()(64bit), libSM.so.6()(64bit), libICE.so.6()(64bit), libfontconfig.so.1()(64bit), libicu, libssl.so.3()(64bit)

%description
ILSpy is the open-source .NET assembly browser and decompiler,
built on Avalonia and the ICSharpCode.Decompiler engine.

%install
mkdir -p %{buildroot}
cp -a %{_payload_dir}/. %{buildroot}/

%files
/opt/ilspy
/usr/bin/ilspy
/usr/share/applications/ilspy.desktop
/usr/share/icons/hicolor/256x256/apps/ilspy.png
