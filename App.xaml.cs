using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace DeskAnim;

public partial class App : Application
{
    private readonly List<OverlayWindow> _overlays = new();
    private Forms.NotifyIcon? _tray;
    private Forms.ToolStripMenuItem? _editItem;
    private MainWindow? _library;

    public bool IsExiting { get; private set; }
    public bool EditMode { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        CreateTray();

        foreach (var s in LayoutStore.Load())
        {
            if (File.Exists(s.FilePath))
                AddOverlay(s.FilePath, s);
        }

        ShowLibrary();
    }

    private void CreateTray()
    {
        _editItem = new Forms.ToolStripMenuItem("Edit mode") { CheckOnClick = true };
        _editItem.CheckedChanged += (_, _) => SetEditMode(_editItem.Checked);

        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("Library", null, (_, _) => ShowLibrary());
        menu.Items.Add(_editItem);
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitApp());

        _tray = new Forms.NotifyIcon
        {
            Icon = Drawing.SystemIcons.Application,
            Text = "DeskAnim",
            Visible = true,
            ContextMenuStrip = menu
        };
        _tray.DoubleClick += (_, _) => ShowLibrary();
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
