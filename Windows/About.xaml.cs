using SaveGameManager.Resources;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Policy;
using System.Windows;


namespace SaveGameManager
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class About : Window
  {
    public About()
    {
      InitializeComponent();
    }

    private void btnDiscord_Click(object sender, RoutedEventArgs e) => OpenUrl(Hyperlinks.Discord);
    private void btnTwitch_Click(object sender, RoutedEventArgs e) => OpenUrl(Hyperlinks.Twitch);
    private void btnYtub_Click(object sender, RoutedEventArgs e) => OpenUrl(Hyperlinks.Youtube);

    private void OpenUrl(string url) 
    {
      try
      {
        Process.Start(new ProcessStartInfo { FileName = @$"{url}", UseShellExecute = true });
      }
      catch
      { MessageBox.Show("Error while opening the Url"); }
    }

    private void wdAbout_Loaded(object sender, RoutedEventArgs e)
    {
      lblVersion.Content = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
    }
  }
}
