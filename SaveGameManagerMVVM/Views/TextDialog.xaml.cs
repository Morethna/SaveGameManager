using System.Windows;

namespace SaveGameManager.Views;

/// <summary>
/// Interaction logic for TextDialog.xaml
/// </summary>
public partial class TextDialog : Window
{
    public TextDialog()
    {
        InitializeComponent();
    }
    private void Window_GotFocus(object sender, RoutedEventArgs e) => txtResponse.Focus();
}
