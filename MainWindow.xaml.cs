using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DeskAnim.Media;
using Microsoft.Win32;

namespace DeskAnim;

public partial class MainWindow : Window
{
    private readonly LocalSource _local = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await RefreshAsync();
    }

    private async Task RefreshAsync() =>
        List.ItemsSource = await _local.SearchAsync(SearchBox.Text);

    private async void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
        await RefreshAsync();

    private async void AddBtn_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Images|*.gif;*.png;*.jpg;*.jpeg;*.bmp"
        };
        if (dlg.ShowDialog() != true) return;

        foreach (var f in dlg.FileNames) _local.Import(f);
        await RefreshAsync();
    }

    private void Place_Click(object sender, RoutedEventArgs e) => PlaceSelected();

    private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e) => PlaceSelected();

    private void PlaceSelected()
    {
        if (List.SelectedItem is MediaItem item)
            ((App)Application.Current).AddOverlay(item.FilePath);
    }

    private void Clear_Click(object sender, RoutedEventArgs e) =>
        ((App)Application.Current).ClearOverlays();

    protected override void OnClosing(CancelEventArgs e)
    {
        // Closing the library hides it to the tray instead of quitting.
        if (!((App)Application.Current).IsExiting)
        {
            e.Cancel = true;
            Hide();
        }
        base.OnClosing(e);
    }
}
