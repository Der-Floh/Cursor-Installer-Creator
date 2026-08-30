# Cursor-Installer-Creator

Cursor Installer Creator builds distributable Windows cursor schemes from individual cursor files. Packaging one by hand means collecting the .cur and .ani files, mapping each one to the system role it should replace, and writing the .inf file that Windows uses to register the scheme — tedious work that is easy to get wrong. This application handles that assembly and produces a self-contained package that anyone can install with a right-click, or apply directly to the machine it is running on. It is intended for people who create or redistribute custom cursor sets.

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

### If you found an error or want to submit an idea feel free to create an [Issue](https://github.com/Der-Floh/Cursor-Installer-Creator/issues/new) or view all [Issues](https://github.com/Der-Floh/Cursor-Installer-Creator/issues)

&nbsp;

[!["Buy me a Floppy Disk"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/der_floh)
