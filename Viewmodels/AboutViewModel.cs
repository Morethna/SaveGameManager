using SaveGameManager.Core;
using SaveGameManager.Resources;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace SaveGameManager.Viewmodels;

public class AboutViewModel : ViewModelBase
{
    public AboutViewModel()
    {
        OpenLinkCommand = new DelegateCommand(OpenUrl);
    }
    public ICommand OpenLinkCommand { get; set; }

    public string Version { get => $"Version: {Assembly.GetExecutingAssembly().GetName().Version}"; }

    private void OpenUrl(object obj)
    {
        string url = obj switch
        {
            "YouTube" => Hyperlinks.Youtube,
            "Discord" => Hyperlinks.Discord,
            _ => Hyperlinks.Twitch
        };
        try
        {
            Process.Start(new ProcessStartInfo { FileName = @$"{url}", UseShellExecute = true });
        }
        catch
        { 
            MessageBox.Show($"Error while opening the {url}");
        }
    }
}
