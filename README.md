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

- MSIX Package: In Visual Studio 2022’s Solution Explorer, select  Clever‑Vpn project, right‑click, choose Package and Publish → Create App Packages, and then follow the on‑screen steps to generate the MSIX package.
- MSI Package: In Visual Studio 2022’s Solution Explorer, select MsiInstaller project, right‑click, choose build to generate the MSI package.


## License

This repository is MIT-licensed. see the [licence.txt](licence.txt) file for details.


