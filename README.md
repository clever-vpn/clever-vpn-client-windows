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
		- `InstallerTagName`
		- `InstallerDownloadBaseUrl`
	- `InstallerTagName` + `CleverVpnInstallerVersion` are used to compose the real MSI download URL in setup bootstrapper.
		- URL pattern (x64): `<InstallerDownloadBaseUrl>/<InstallerTagName>/CleverVPN_<CleverVpnInstallerVersion>_x64.msi`
		- example:
			- `InstallerDownloadBaseUrl=https://github.com/clever-vpn/clever-vpn-client-windows/releases/download`
			- `InstallerTagName=v1.3.7-rc.1`
			- `CleverVpnInstallerVersion=1.3.7.0`
			- final x64 URL: `https://github.com/clever-vpn/clever-vpn-client-windows/releases/download/v1.3.7-rc.1/CleverVPN_1.3.7.0_x64.msi`
	- select `SetupInstaller` project, and build,
	- output setup will be generated in `SetupInstaller\bin\<configuration>` and will download/install the matching MSI by architecture.

### GitHub Actions Release Modes

The repository workflow `.github/workflows/release.yml` supports three build/release modes:

- `internal`
	- intended for CI validation only, does not publish a GitHub Release.
	- requires an RC tag format: `v<major>.<minor>.<patch>-rc.<n>`.
	- automatically triggered when an RC tag is pushed (for example `v1.0.0-rc.17`).
	- can also be triggered manually via `workflow_dispatch`.

- `prerelease`
	- publishes a GitHub prerelease.
	- requires an RC tag format: `v<major>.<minor>.<patch>-rc.<n>`.
	- trigger method: `workflow_dispatch`.

- `release`
	- publishes a normal GitHub Release.
	- requires a stable tag format: `v<major>.<minor>.<patch>`.
	- trigger method: `workflow_dispatch`.

#### Manual Trigger Examples

- prerelease:
	- `gh workflow run release.yml -R clever-vpn/clever-vpn-client-windows --ref main -f release_mode=prerelease -f tag_name=v1.0.0-rc.17`

- release:
	- `gh workflow run release.yml -R clever-vpn/clever-vpn-client-windows --ref main -f release_mode=release -f tag_name=v1.0.0`

#### Workflow Outputs and Artifacts

- installer version is resolved from tag and mapped to `major.minor.patch.0`.
- workflow artifacts are set to `retention-days: 7`.
- GitHub Release assets include:
	- `*.msi` (x86/x64/arm64)
	- `*.msixbundle`
	- `setup.exe`
- individual `*.msix` files are intermediate build outputs used for bundle creation and are not published as release assets.


## License

This repository is MIT-licensed. see the [licence.txt](licence.txt) file for details.


