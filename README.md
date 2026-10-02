# DeskAnim

Free, open-source desktop animation overlay for Windows (GIFs and images on your desktop).

## Run
Requires the .NET 8 SDK.

    dotnet run

## Usage
- Add files to the library, double-click an item to place it on the desktop.
- Tray icon -> **Edit mode**: drag to move, mouse wheel to scale, right click to remove.
- Outside edit mode, layers are click-through. Layout is saved to `%AppData%/DeskAnim/layers.json`.

## Roadmap
- [ ] Video with alpha (WebM), WEBP/APNG, sprite sheets
- [ ] Giphy/Tenor source (user-provided API key)
- [ ] Bundled CC0 examples
- [ ] Local background removal (ONNX Runtime)
- [ ] Autostart, installer

## License
MIT (code only; see `assets/LICENSES.md` for bundled assets).
