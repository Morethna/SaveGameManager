using System.Windows;

namespace SaveGameManager.Views;

/// <summary>
/// Interaction logic for NotifyBox.xaml
/// </summary>
public partial class NotifyBoxYesNo : Window
{
    public NotifyBoxYesNo()
    {
        InitializeComponent();
    }

    private void wdDialog_GotFocus(object sender, RoutedEventArgs e) => btnNo.Focus();
}
