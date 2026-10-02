# DeskAnim

Free, open-source desktop animation overlay for Windows (GIFs and images on your desktop).

## Install
Download `DeskAnim-Setup-x.y.z.exe` from Releases. The installer adds a desktop shortcut (optional autostart)
and registers an uninstaller (Settings > Apps, or Start menu).

## Build
Requires the .NET 8 SDK.

    dotnet run

Build the installer (also needs Inno Setup 6):

    build-installer.bat

Output: `dist\DeskAnim-Setup-0.1.0.exe`. Pushing a `v*` tag builds and attaches it to a GitHub release automatically.

## Usage
- Add files to the library, double-click an item to place it on the desktop.
- Tray icon: **Edit mode** (drag to move, drag the blue corner handle or use the mouse wheel to resize, right click to remove) and **Dark theme**.
- Video (mp4/H.264 works best) plays muted and looped; no transparency yet.
- Outside edit mode, layers are click-through. Data lives in `%AppData%/DeskAnim`.

## Roadmap
- [ ] Video with alpha (WebM), WEBP/APNG, sprite sheets
- [ ] Giphy/Tenor source (user-provided API key)
- [ ] Bundled CC0 examples
- [ ] Local background removal (ONNX Runtime)

## License
MIT (code only; see `assets/LICENSES.md` for bundled assets).
