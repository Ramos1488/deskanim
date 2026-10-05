using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        SourceInitialized += (_, _) => Native.UseDarkTitleBar(this, ((App)Application.Current).DarkTheme);
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
            Filter = "Images and video|*.gif;*.png;*.jpg;*.jpeg;*.bmp;*.mp4;*.wmv;*.avi;*.mov;*.webm;*.mkv"
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

    private void Delete_Click(object sender, RoutedEventArgs e) => DeleteSelected();

    private void List_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete) DeleteSelected();
    }

    private async void DeleteSelected()
    {
        var items = List.SelectedItems.OfType<MediaItem>().ToList();
        if (items.Count == 0) return;

        var answer = MessageBox.Show(
            $"Delete {items.Count} file(s) from the library?", "DeskAnim",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) return;

        var app = (App)Application.Current;
        foreach (var item in items)
        {
            app.RemoveOverlaysByFile(item.FilePath); // stop showing it on the desktop first
            if (!TryDelete(item.FilePath))
                MessageBox.Show($"Could not delete {System.IO.Path.GetFileName(item.FilePath)}.", "DeskAnim");
        }
        await RefreshAsync();
    }

    private static bool TryDelete(string path)
    {
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (IOException)
            {
                // The file may still be held by a just-closed overlay: release it and retry.
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
        return !File.Exists(path);
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
