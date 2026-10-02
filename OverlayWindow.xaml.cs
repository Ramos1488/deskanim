using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace DeskAnim;

public partial class OverlayWindow : Window
{
    private readonly string _filePath;
    private bool _editMode;
    private double _scale = 1.0;

    public OverlayWindow(string filePath, LayerState? state)
    {
        InitializeComponent();
        _filePath = filePath;

        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(filePath);
        bmp.EndInit();

        Img.Width = bmp.Width;
        Img.Height = bmp.Height;
        ImageBehavior.SetAnimatedSource(Img, bmp);

        Left = state?.Left ?? 100;
        Top = state?.Top ?? 100;
        Scale = state?.Scale ?? 1.0;

        SourceInitialized += (_, _) => Native.HideFromAltTab(this);
    }

    public double Scale
    {
        get => _scale;
        set
        {
            _scale = Math.Clamp(value, 0.1, 5.0);
            Img.LayoutTransform = new ScaleTransform(_scale, _scale);
        }
    }

    public void SetEditMode(bool on)
    {
        _editMode = on;
        Frame.BorderBrush = on ? Brushes.DeepSkyBlue : Brushes.Transparent;
        Frame.Background = on
            ? new SolidColorBrush(Color.FromArgb(32, 0, 120, 255))
            : Brushes.Transparent;
        Native.SetClickThrough(this, !on);
    }

    public LayerState ToState() => new()
    {
        FilePath = _filePath,
        Left = Left,
        Top = Top,
        Scale = Scale
    };

    // Edit mode: drag = move, wheel = scale, right click = remove.
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (_editMode) DragMove();
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        if (_editMode) Scale *= e.Delta > 0 ? 1.1 : 1 / 1.1;
    }

    protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseRightButtonUp(e);
        if (_editMode) ((App)Application.Current).RemoveOverlay(this);
    }
}
