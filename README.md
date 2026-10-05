# DeskAnim

Free, open-source desktop animation overlay for Windows. Put GIFs, images and videos on top of your desktop, move and resize them, and keep your layout between launches.

## Features

- Animated GIFs, images (PNG, JPG, BMP) and videos (MP4, WMV, AVI, MOV, WEBM, MKV) displayed above other windows
- Click-through: the animations never get in your way
- Edit mode: drag to move, drag the corner handle or use the mouse wheel to resize, right-click to remove
- Library with search; add files, delete files you no longer need
- Layout is saved and restored on the next launch
- Dark theme (default) and light theme
- Tray icon: Library, Edit mode, Dark theme, Exit
- Single instance: running it twice won't create a second copy

## Download and run

1. Go to the [Releases](../../releases) page and download the latest `.zip`.
2. Extract it to any folder (for example `C:\Apps\DeskAnim`). Don't run it from inside the zip.
3. Run `DeskAnim.exe`.

**Requirements:** Windows 10 or 11, 64-bit. If Windows says a .NET runtime is missing, install the
[.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0).

> The app is not code-signed yet, so Windows SmartScreen may show "Windows protected your PC".
> Click **More info → Run anyway**. Some antivirus programs can also flag unsigned apps: the whole
> source code is in this repository, and you can build the app yourself (see below).

To verify the download, compare the SHA-256 with the one in the release notes:

```
certutil -hashfile DeskAnim-v1.1.0-win-x64.zip SHA256
```

## How to use

1. Click **Add files…** in the library window and pick GIFs, images or videos. They are copied into the library.
2. Select an item and double-click it (or press **Place on desktop**). It appears on your desktop in Edit mode.
3. In **Edit mode** (also in the tray menu):
   - drag the animation to move it
   - drag the blue corner handle or scroll the mouse wheel to resize it
   - right-click it to remove it from the desktop
4. Turn Edit mode off in the tray menu: the layout is saved and animations become click-through.
5. To delete a file from the library, select it and press **Delete** (Ctrl/Shift for several files).

Closing the library window hides it to the tray. Use **Exit** in the tray menu to quit.

## Where your data is stored

Everything is in `%AppData%\DeskAnim`:

| File | Content |
| --- | --- |
| `library\` | files you added |
| `layers.json` | positions and sizes of animations on the desktop |
| `settings.json` | theme |

## Uninstall

There is no installer: choose **Exit** in the tray menu and delete the folder you extracted.
To remove your library and settings too, delete `%AppData%\DeskAnim`.

## Notes and limitations

- Videos play muted and looped, without transparency. MP4 (H.264) is the most reliable format; other formats depend on codecs installed in Windows.
- Not supported yet: WEBP/APNG, online GIF search (Giphy/Tenor), background removal.

## Build from source

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) on Windows.

Run it:

```
dotnet run
```

Make a zip you can share (works without installing .NET):

```
dotnet publish DeskAnim.csproj -c Release -r win-x64 --self-contained true -o publish
powershell Compress-Archive -Path publish\* -DestinationPath DeskAnim-v1.1.0-win-x64.zip
```

Optional: `build-installer.bat` builds a Windows installer with [Inno Setup 6](https://jrsoftware.org/isdl.php).

## Roadmap

- [ ] Giphy/Tenor search (with your own API key)
- [ ] Bundled CC0 example animations
- [ ] WEBP/APNG, video with transparency
- [ ] Local background removal
- [ ] Code signing

## License

MIT. Bundled assets, if any, keep their own licenses (see `assets/LICENSES.md`).
