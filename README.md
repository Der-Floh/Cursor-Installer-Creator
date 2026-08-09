# Cursor-Installer-Creator

A program to create Cursor-Installer files simply and fast.

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/Der-Floh/Cursor-Installer-Creator/main/Cursor_Installer_Creator/Resources/preview-dark.png">
  <img alt="Shows a preview of the Cursor-Installer-Creator" src="https://raw.githubusercontent.com/Der-Floh/Cursor-Installer-Creator/main/Cursor_Installer_Creator/Resources/preview-light.png">
</picture>

### [Download Latest Release](https://github.com/Der-Floh/Cursor-Installer-Creator/releases/latest)

### [Use Web Version](https://der-floh.github.io/Cursor-Installer-Creator/)

## Usage

Pick or Drag & Drop cursor files in their corresponding slot. Give the cursor package a name and hit create package. Optionally you can also decide wether to create the package inside a .zip compressed archive or just inside a folder.

## Features

- Pick .cur and .ani cursor files
- Drag and Drop .cur and .ani files
- Create Installer-Package as Folder or .zip archive
- Directly install a new cursor from the program
- Reset individual cursors to default
- Import Cursor-Installer (.inf) File
- Preview Cursors
- Light and Dark Mode

## Security & Code Signing

Official Windows release binaries — the `.exe` inside the portable ZIP and the `.msi` installer — are digitally signed.

#### Code signing policy

- Only binaries built from this repository's source by the official GitHub Actions release workflow ([`.github/workflows/publish.yml`](.github/workflows/publish.yml)) are signed.
- Every release is signed through [SignPath.io](https://signpath.io) and requires **manual approval by a project Approver** before signing takes place.
- The code signing certificate is issued by the [SignPath Foundation](https://signpath.org).
- Unofficial builds, forks, and locally compiled binaries are **not** signed and will show an "unknown publisher" prompt.

#### System changes made by the app

- Choosing **Install Cursor** copies the selected cursor files into `%WINDIR%\Cursors` and updates the current user's cursor scheme under `HKEY_CURRENT_USER\Control Panel\Cursors`. This requires administrator approval (UAC) and only happens when you explicitly start an install.
- Creating a package (folder, ZIP, or installer) only writes to the location you choose and makes **no** system changes.

#### Project team & roles

| Member   | GitHub                                   | Roles                      |
| -------- | ---------------------------------------- | -------------------------- |
| Der_Floh | [@Der-Floh](https://github.com/Der-Floh) | Author, Reviewer, Approver |

## Privacy Policy

Cursor Installer Creator does not collect, store, or transmit any personal data.

- **Update check:** On Windows, the desktop app contacts the public GitHub Releases API (`https://api.github.com/repos/Der-Floh/Cursor-Installer-Creator/releases/latest`) once at startup to compare your installed version against the latest release. The request sends only a generic `User-Agent` header — no personal data, telemetry, or analytics.
- **No accounts or tracking:** The app has no user accounts, ads, or third-party analytics.
- **Opt out / offline use:** The app is fully functional offline. The update check is the only outbound request the app makes; you can prevent it by blocking network access to `api.github.com`. The [web version](https://der-floh.github.io/Cursor-Installer-Creator/) performs no update check.

## Credits

Free code signing for this open-source project is generously provided by the [SignPath Foundation](https://signpath.org), with code signing infrastructure by [SignPath.io](https://signpath.io).

&nbsp;

### If you found an error or want to submit an idea feel free to create an [Issue](https://github.com/Der-Floh/Cursor-Installer-Creator/issues/new) or view all [Issues](https://github.com/Der-Floh/Cursor-Installer-Creator/issues)

&nbsp;

[!["Buy me a Floppy Disk"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/der_floh)
