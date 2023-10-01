using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SaveGameManager
{
  /// <summary>
  /// Interaction logic for TextDialog.xaml
  /// </summary>
  public partial class TextDialog : Window
  {
    public TextDialog(string profile)
    {
      InitializeComponent();
      txtResponse.Text = profile;
    }
    public string ResponseText { get => txtResponse.Text; set => txtResponse.Text = value; }
    private void btnOk_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
  }
}
