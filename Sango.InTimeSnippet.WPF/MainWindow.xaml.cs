using NHotkey;
using NHotkey.Wpf;

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sango.InTimeSnippet.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public KeyGesture _showHideGesture
    {
        get; private set;
    } = new KeyGesture(Key.L, ModifierKeys.Alt);

    public KeyGesture _turnLaTeXGesture
    {
        get; private set;
    } = new KeyGesture(Key.K, ModifierKeys.Alt);

    public MainWindow()
    {
        InitializeComponent();
        RegisterGlobalHotkey();
    }

    private void TurnVisibility()
    {
        if (IsVisible)
        {
            Hide();
        }
        else
        {
            Show();
            Focus();
        }
    }

    private void RegisterGlobalHotkey()
    {
        try
        {
            HotkeyManager.Current.AddOrReplace("ShowHide", _showHideGesture, (s, e) =>
            {
                TurnVisibility();
                e.Handled = true;
            });
            HotkeyManager.Current.AddOrReplace("TurnLaTeX", _turnLaTeXGesture, (s, e) =>
            {
                SnippetBox.TurnLaTeX();
                e.Handled = true;
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"注册热键失败: {ex.Message}");
        }
    }

    private void MouseDragMove(MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        MouseDragMove(e);
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        MouseDragMove(e);
    }
}