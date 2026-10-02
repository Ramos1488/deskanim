using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DeskAnim.Media;
using WpfAnimatedGif;

namespace DeskAnim;

public partial class OverlayWindow : Window
{
    private readonly string _filePath;
    private bool _editMode;
    private double _scale = 1.0;
    private double _baseWidth = 1;
    private double _baseHeight = 1;

    private bool _resizing;
    private Point _resizeStartScreen;
    private double _resizeStartWidth;

    public OverlayWindow(string filePath, LayerState? state)
    {
        InitializeComponent();
        _filePath = filePath;

        if (MediaKinds.IsVideo(filePath)) BuildVideo(filePath);
        else BuildImage(filePath);

        Left = state?.Left ?? 100;
        Top = state?.Top ?? 100;
        Scale = state?.Scale ?? 1.0;

        SourceInitialized += (_, _) => Native.HideFromAltTab(this);
        Closed += (_, _) =>
        {
            if (Host.Children.Count > 0 && Host.Children[0] is MediaElement m)
            {
                m.Stop();
                m.Source = null;
            }
        };
    }

    private void BuildImage(string filePath)
    {
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(filePath);
        bmp.EndInit();

        var img = new Image { Stretch = Stretch.Uniform };
        RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
        ImageBehavior.SetAnimatedSource(img, bmp);
        Host.Children.Add(img);

        SetBaseSize(bmp.Width, bmp.Height);
    }

    private void BuildVideo(string filePath)
    {
        var video = new MediaElement
        {
            LoadedBehavior = MediaState.Manual,
            UnloadedBehavior = MediaState.Manual,
            Volume = 0, // muted: it's a desktop decoration
            Stretch = Stretch.Uniform,
            Source = new Uri(filePath)
        };
        video.MediaOpened += (_, _) =>
        {
            if (video.NaturalVideoWidth > 0 && video.NaturalVideoHeight > 0)
                SetBaseSize(video.NaturalVideoWidth, video.NaturalVideoHeight);
        };
        video.MediaEnded += (_, _) =>
        {
            video.Position = TimeSpan.Zero;
            video.Play();
        };
        Host.Children.Add(video);

        SetBaseSize(320, 180); // placeholder until the video reports its real size
        video.Play();
    }

    private void SetBaseSize(double width, double height)
    {
        _baseWidth = Math.Max(1, width);
        _baseHeight = Math.Max(1, height);
        Host.Width = _baseWidth;
        Host.Height = _baseHeight;
    }

    public double Scale
    {
        get => _scale;
        set
        {
            _scale = Math.Clamp(value, 0.1, 5.0);
            Host.LayoutTransform = new ScaleTransform(_scale, _scale);
        }
    }

    public void SetEditMode(bool on)
    {
        _editMode = on;
        Frame.BorderBrush = on ? Brushes.DeepSkyBlue : Brushes.Transparent;
        Frame.Background = on
            ? new SolidColorBrush(Color.FromArgb(32, 0, 120, 255))
            : Brushes.Transparent;
        Grip.Visibility = on ? Visibility.Visible : Visibility.Collapsed;
        Native.SetClickThrough(this, !on);
    }

    public LayerState ToState() => new()
    {
        FilePath = _filePath,
        Left = Left,
        Top = Top,
        Scale = Scale
    };

    // Edit mode: drag = move, corner handle / wheel = resize, right click = remove.
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (_editMode && !_resizing) DragMove();
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

    // Corner handle: keeps the aspect ratio. Uses screen coordinates so the
    // window resizing under the cursor doesn't cause feedback jitter.
    private Point ScreenPos(MouseEventArgs e) => PointToScreen(e.GetPosition(this));

    private void Grip_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _resizing = true;
        _resizeStartScreen = ScreenPos(e);
        _resizeStartWidth = _baseWidth * Scale;
        Grip.CaptureMouse();
        e.Handled = true;
    }

    private void Grip_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_resizing) return;
        double dpi = VisualTreeHelper.GetDpi(this).DpiScaleX;
        double dx = (ScreenPos(e).X - _resizeStartScreen.X) / dpi;
        Scale = (_resizeStartWidth + dx) / _baseWidth;
    }

    private void Grip_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _resizing = false;
        Grip.ReleaseMouseCapture();
        e.Handled = true;
    }
}
