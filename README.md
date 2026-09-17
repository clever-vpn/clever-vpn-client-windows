# [Clever VPN](https://www.clever-vpn.net/) for Windows
![License](https://img.shields.io/badge/license-MIT-blue.svg)

## Overview

The Clever VPN Windows Client is an open-source application built on WinUI 3, the latest Windows UI framework. It offers users a lightweight and user-friendly VPN connection experience. The core VPN protocol implementations are encapsulated in the [Clever-Vpn-Windows-Kit](https://www.nuget.org/packages/Clever-Vpn-Windows-Kit) library, reducing client-side code complexity and facilitating easy customization and extension.

## Key Features

- **Modern UI**: Built with WinUI 3, it supports native look-and-feel and animations on Windows 10 and 11.
- **Simple Architecture**: Client logic is straightforward, making it easy to read and maintain.
- **Modular Design**: VPN protocol implementations are separated from the UI and packaged in Clever-Vpn-Windows-Kit.
- **Multiple Distribution Options**: Offers both MSIX and MSI installers for Microsoft Store distribution or standalone installation.
- **Plug-and-Play**: Download, install in one click, and connect quickly.


## Building & Packaging

### Development Environment

Before building the project locally, ensure you have the following installed:

- Visual Studio 2022 with the Desktop Development with C++ and .NET Desktop Development workloads.
- .NET 8 SDK
- Windows App SDK 1.7 or later

### Packaging

- MSIX Package: In Visual Studio 2022’s Solution Explorer, follow these steps to create an MSIX package:
	- select Clever-Vpn project, right‑click,
	- right‑click, choose build to generate the MSIX package.
	- after create msix package, you can run make-bundle.bat to generate the bundle package.

- MSI Package: In Visual Studio 2022’s Solution Explorer, follow these steps to create architecture-specific MSI packages:
	- select solution platform, for example, x64, x86, or arm64,
	- select MsiInstaller project, and right-click,
	- choose build to generate the MSI package,
	- output file names are unified as `CleverVPN_<version>_<platform>.msi` in `MsiInstaller\bin\x64|x86|ARM64\Release`.

- Unified Setup EXE (auto architecture selection):
	- configure `InstallerSettings.props` at solution root:
		- `CleverVpnInstallerVersion`
		- `InstallerDownloadBaseUrl`
	- `InstallerDownloadBaseUrl` + `CleverVpnInstallerVersion` compose the real MSI download URL in the setup bootstrapper.
		- URL pattern (x64): `<InstallerDownloadBaseUrl>/CleverVPN_<CleverVpnInstallerVersion>_x64.msi`
		- example: `InstallerDownloadBaseUrl=https://download.clever-vpn.net/windows` and `CleverVpnInstallerVersion=1.3.7.0` give `https://download.clever-vpn.net/windows/CleverVPN_1.3.7.0_x64.msi`
	- the URL is baked into the bootstrapper at build time: `setup.exe` carries no application payload and does no mode detection at run time, so editing `InstallerSettings.props` only affects local builds.
	- each release mode therefore passes its own `-p:InstallerDownloadBaseUrl`:
		- `release` → `https://download.clever-vpn.net/windows` (the release pipeline publishes the MSIs there), plus an extra `CleverVPN_Setup_Test.exe` built with `https://download-test.clever-vpn.net/windows`
		- `prerelease` → `https://github.com/<owner>/<repo>/releases/download/<tag>`, so the `setup.exe` shipped in a prerelease installs the MSIs of that very release
	- select `SetupInstaller` project, and build,
	- output setup will be generated in `SetupInstaller\bin\<configuration>` and will download/install the matching MSI by architecture.

### GitHub Actions Workflows

A release is driven by a **tag**, a test build is driven by a **commit**, so the two live in separate workflows. Both call `.github/workflows/_build-installers.yml`, which is the single place where the MSI / MSIX / bundle / setup installers are built.

#### `.github/workflows/release.yml` (tag driven, publishes a GitHub Release)

- `prerelease`
	- publishes a GitHub prerelease.
	- requires an RC tag format: `v<major>.<minor>.<patch>-rc.<n>`.
	- its `setup.exe` downloads the MSIs from the assets of that same release, because a prerelease is not published to the download host. No `CleverVPN_Setup_Test.exe` is produced.

- `release`
	- publishes a normal GitHub Release.
	- requires a stable tag format: `v<major>.<minor>.<patch>`.
	- pass `tag_name` empty to auto-bump the latest release patch.

- trigger method: `workflow_dispatch`.

#### `.github/workflows/internal.yml` (ref driven, publishes nothing)

- builds any branch, tag or commit of this repository, for real-device testing.
- inputs:
	- `ref` (default `main`): the branch, tag or commit to build, for example `test/kit-2.1.3-rc.1`.
	- `version_base` (optional): the `<major>.<minor>` part of the test installer version; by default it is taken from the latest stable release.
- no tag and no GitHub Release are created: the installers stay as artifacts of the run (`retention-days: 7`).
- test installer version is `<major>.<minor>.<run_number>.0`. The build field comes from the run number on purpose: the MSI refuses a downgrade, so the test build must compare as newer than any published release. Uninstall a test build before installing an older or equal version.
- `setup.exe` is not built here: it downloads its MSIs from the download base URL, which this workflow does not publish to.

#### Manual Trigger Examples

- internal test build of a branch that pins a prerelease kit:
	- `gh workflow run internal.yml -R clever-vpn/clever-vpn-client-windows --ref main -f ref=test/kit-2.1.3-rc.1`

- prerelease:
	- `gh workflow run release.yml -R clever-vpn/clever-vpn-client-windows --ref main -f release_mode=prerelease -f tag_name=v1.0.0-rc.17`

- release:
	- `gh workflow run release.yml -R clever-vpn/clever-vpn-client-windows --ref main -f release_mode=release -f tag_name=v1.0.0`

#### Workflow Outputs and Artifacts

- the installer version of a release is resolved from the tag and mapped to `major.minor.patch.0`.
- artifacts of a release run are `retention-days: 1`: they are only the transport between the build jobs and the publishing job of the same run, and the Release assets are the long lived copies.
- GitHub Release assets include:
	- `*.msi` (x86/x64/arm64)
	- `*.msixbundle`
	- `setup.exe`
- individual `*.msix` files are intermediate build outputs used for bundle creation and are not published as release assets.


## License

This repository is MIT-licensed. see the [licence.txt](licence.txt) file for details.


