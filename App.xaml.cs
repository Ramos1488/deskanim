using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace DeskAnim;

public partial class App : Application
{
    private const string MutexName = "DeskAnim.SingleInstance";
    private static Mutex? _mutex;

    private readonly List<OverlayWindow> _overlays = new();
    private AppSettings _settings = new();
    private Forms.NotifyIcon? _tray;
    private Forms.ToolStripMenuItem? _editItem;
    private MainWindow? _library;

    public bool IsExiting { get; private set; }
    public bool EditMode { get; private set; }
    public bool DarkTheme => _settings.DarkTheme;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mutex = new Mutex(true, MutexName, out bool created);
        if (!created)
        {
            Shutdown();
            return;
        }

        _settings = SettingsStore.Load();
        ApplyTheme(_settings.DarkTheme);
        CreateTray();

        foreach (var s in LayoutStore.Load())
        {
            if (File.Exists(s.FilePath))
                AddOverlay(s.FilePath, s);
        }

        ShowLibrary();
    }

    private void ApplyTheme(bool dark)
    {
        Resources.MergedDictionaries[0] = new ResourceDictionary
        {
            Source = new Uri(dark ? "Themes/Dark.xaml" : "Themes/Light.xaml", UriKind.Relative)
        };
        if (_library != null) Native.UseDarkTitleBar(_library, dark);
        ApplyTrayTheme(dark);
    }

    private void CreateTray()
    {
        _editItem = new Forms.ToolStripMenuItem("Edit mode") { CheckOnClick = true };
        _editItem.CheckedChanged += (_, _) => SetEditMode(_editItem.Checked);

        var darkItem = new Forms.ToolStripMenuItem("Dark theme")
        {
            CheckOnClick = true,
            Checked = _settings.DarkTheme
        };
        darkItem.CheckedChanged += (_, _) =>
        {
            _settings.DarkTheme = darkItem.Checked;
            SettingsStore.Save(_settings);
            ApplyTheme(_settings.DarkTheme);
        };

        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("Library", null, (_, _) => ShowLibrary());
        menu.Items.Add(_editItem);
        menu.Items.Add(darkItem);
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitApp());

        Drawing.Icon icon = Drawing.SystemIcons.Application;
        try
        {
            if (Environment.ProcessPath is { } exe)
                icon = Drawing.Icon.ExtractAssociatedIcon(exe) ?? icon;
        }
        catch (Exception)
        {
            // Fall back to the default icon.
        }

        _tray = new Forms.NotifyIcon
        {
            Icon = icon,
            Text = "DeskAnim",
            Visible = true,
            ContextMenuStrip = menu
        };
        _tray.DoubleClick += (_, _) => ShowLibrary();
        ApplyTrayTheme(_settings.DarkTheme);
    }

    private void ApplyTrayTheme(bool dark)
    {
        if (_tray?.ContextMenuStrip is not { } menu) return;
        menu.Renderer = dark
            ? new Forms.ToolStripProfessionalRenderer(new DarkColors())
            : new Forms.ToolStripProfessionalRenderer();
        menu.ForeColor = dark ? Drawing.Color.FromArgb(0xE8, 0xE8, 0xEE) : Drawing.SystemColors.ControlText;
        menu.BackColor = dark ? Drawing.Color.FromArgb(0x2A, 0x2A, 0x32) : Drawing.SystemColors.Menu;
    }

    public void ShowLibrary()
    {
        _library ??= new MainWindow();
        _library.Show();
        _library.Activate();
    }

    public void AddOverlay(string filePath, LayerState? state = null)
    {
        var w = new OverlayWindow(filePath, state);
        _overlays.Add(w);
        w.Show();

        // New layers start in edit mode so the user can place them right away.
        if (state == null) SetEditMode(true);
        w.SetEditMode(EditMode);
    }

    public void RemoveOverlay(OverlayWindow w)
    {
        _overlays.Remove(w);
        w.Close();
        SaveLayout();
    }

    public void RemoveOverlaysByFile(string filePath)
    {
        foreach (var w in _overlays
            .Where(o => string.Equals(o.FilePath, filePath, StringComparison.OrdinalIgnoreCase))
            .ToList())
        {
            _overlays.Remove(w);
            w.Close();
        }
        SaveLayout();
    }

    public void ClearOverlays()
    {
        foreach (var w in _overlays.ToList()) w.Close();
        _overlays.Clear();
        SaveLayout();
    }

    public void SetEditMode(bool on)
    {
        if (EditMode == on) return;
        EditMode = on;
        foreach (var w in _overlays) w.SetEditMode(on);
        if (_editItem != null) _editItem.Checked = on;
        if (!on) SaveLayout();
    }

    public void SaveLayout() =>
        LayoutStore.Save(_overlays.Select(o => o.ToState()));

    public void ExitApp()
    {
        IsExiting = true;
        SaveLayout();
        if (_tray != null)
        {
            _tray.Visible = false;
            _tray.Dispose();
        }
        foreach (var w in _overlays.ToList()) w.Close();
        _library?.Close();
        Shutdown();
    }
}
