using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SaveGameManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private static TreeViewItem VisualUpwardSearch(DependencyObject source)
    {
        while (source != null && !(source is TreeViewItem))
            source = VisualTreeHelper.GetParent(source);

        return source as TreeViewItem;
    }
    private void tvSavegame_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

        if (treeViewItem != null)
        {
            treeViewItem.Focus();
            e.Handled = true;
        }
    }
}